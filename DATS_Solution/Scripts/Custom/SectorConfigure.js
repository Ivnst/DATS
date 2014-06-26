//some links
//http://blog.mariusschulz.com/2014/02/05/passing-net-server-side-data-to-javascript
//http://simonsarris.com/blog/510-making-html5-canvas-useful
//http://www.w3schools.com/tags/ref_canvas.asp

// Constructor for Shape objects to hold data for all drawn objects.
// For now they will just be defined as rectangles.
function Shape(row, col, state, selected) {
    // This is a very simple and unsafe constructor. All we're doing is checking if the values exist.
    // "x || 0" just means "if there is a value for x, use that. Otherwise use 0."
    // But we aren't checking anything else! We could put "Lalala" for the value of x 
    //this.x = x || 0;
    //this.y = y || 0;
    //this.w = w || 1;
    //this.h = h || 1;
    this.row = row || 0;
    this.col = col || 0;
    this.state = state || 0;
    this.selected = selected || 0;
    this.label = '';
}

// Determine if a point is inside the shape's bounds
// Shape.prototype.contains = function (mx, my) {
//     // All we have to do is make sure the Mouse X,Y fall in the area between
//     // the shape's X and (X + Height) and its Y and (Y + Height)
//     return (this.x <= mx) && (this.x + this.w >= mx) &&
//           (this.y <= my) && (this.y + this.h >= my);
// }

function CanvasState(canvas, width, height) {
    // **** First some setup! ****

    this.itemWidth = 25;
    this.itemHeight = 25;
    this.maxRows = 20;
    this.maxCols = 30;
    this.offsetX = 0;
    this.offsetY = 0;

    this.canvas = canvas;
    //настройка ширины канвы
    this.width = this.canvas.width = width;
    this.height = this.canvas.height = height;

    this.ctx = canvas.getContext('2d');

    // This complicates things a little but but fixes mouse co-ordinate problems
    // when there's a border or padding. See getMouse for more detail
    var stylePaddingLeft, stylePaddingTop, styleBorderLeft, styleBorderTop;
    if (document.defaultView && document.defaultView.getComputedStyle) {
        this.stylePaddingLeft = parseInt(document.defaultView.getComputedStyle(canvas, null)['paddingLeft'], 10) || 0;
        this.stylePaddingTop = parseInt(document.defaultView.getComputedStyle(canvas, null)['paddingTop'], 10) || 0;
        this.styleBorderLeft = parseInt(document.defaultView.getComputedStyle(canvas, null)['borderLeftWidth'], 10) || 0;
        this.styleBorderTop = parseInt(document.defaultView.getComputedStyle(canvas, null)['borderTopWidth'], 10) || 0;
    }

    // Some pages have fixed-position bars (like the stumbleupon bar) at the top or left of the page
    // They will mess up mouse coordinates and this fixes that
    var html = document.body.parentNode;
    this.htmlTop = html.offsetTop;
    this.htmlLeft = html.offsetLeft;

    // **** Keep track of state! ****

    // the collection of things to be drawn
    this.shapes = [];
    for(var i = 0; i < this.maxRows; i++){
      this.shapes[i] = [];
    }

    for (var i = 0; i < this.maxRows; i++) {
        for (var j = 0; j < this.maxCols; j++) {
            this.shapes[i][j] = new Shape(i, j, false, false);
        }
    }

    this.valid = false; // when set to false, the canvas will redraw everything
    //this.dragging = false; // Keep track of when we are dragging
    // the current selected object. In the future we could turn this into an array for multiple selection
    this.selection = false; //
    this.initialX = 0; // начальное положение указателя мыши при выделении
    this.initialY = 0;
    this.selectionX = 0;
    this.selectionY = 0;   //текущее положение указателя мыши во время выделения
    this.mousePressed = false; //нажата кнопка мыши
    this.firstSelectedItem = null;

    // **** Then events! ****

    // This is an example of a closure!
    // Right here "this" means the CanvasState. But we are making events on the Canvas itself,
    // and when the events are fired on the canvas the variable "this" is going to mean the canvas!
    // Since we still want to use this particular CanvasState in the events we have to save a reference to it.
    // This is our reference!
    var myState = this;

    //fixes a problem where double clicking causes text to get selected on the canvas
    canvas.addEventListener('selectstart', function (e) { e.preventDefault(); return false; }, false);

    // Up, down, and move are for dragging
    canvas.addEventListener('mousedown', function (e) {
        var mouse = myState.getMouse(e);
        myState.initialX = mouse.x;
        myState.initialY = mouse.y;
        myState.mousePressed = true;
        myState.firstSelectedItem = myState.getItem(mouse.x, mouse.y);

    }, true);

    canvas.addEventListener('mousemove', function (e) {
        if (!myState.mousePressed) return;
        var mouse = myState.getMouse(e);
        var itm = myState.getItem(mouse.x, mouse.y);
        if (itm.row == myState.firstSelectedItem.row && itm.col == myState.firstSelectedItem.col && !myState.selection) return;
        myState.selection = true;
        myState.selectionX = mouse.x;
        myState.selectionY = mouse.y;

        //set selection
        for (var i = 0; i < myState.maxRows; i++) {
            for (var j = 0; j < myState.maxCols; j++) {
                var shape = myState.shapes[i][j];
                shape.selected = myState.checkIntersection(shape);
            }
        }

        myState.valid = false; // Something's dragging so we must redraw
    }, true);

    canvas.addEventListener('mouseup', function (e) {
        if (!myState.selection) {
            // myState.clearSelection();
            var mouse = myState.getMouse(e);
            var itm = myState.getItem(mouse.x, mouse.y);

            //change selection
            var shape = myState.shapes[itm.row][itm.col];
            shape.selected = !shape.selected;
        }
        myState.selection = false;
        myState.mousePressed = false;
        myState.firstSelectedItem = null;
        myState.valid = false; // Need to clear the old selection border

    }, true);

    // double click for making new shapes
    canvas.addEventListener('dblclick', function (e) {
//         var mouse = myState.getMouse(e);
//         myState.addShape(new Shape(mouse.x - 10, mouse.y - 10, 20, 20, 'rgba(0,255,0,.6)'));
    }, true);

    // **** Options! ****

    this.selectionColor = '#CC0000';
    this.selectionWidth = 2;
    this.interval = 30;
    setInterval(function () { myState.draw(); }, myState.interval);
}

CanvasState.prototype.addShape = function (row, col, state) {
    var shape = this.shapes[row][col];
    shape.state = state;
    this.valid = false;
}

CanvasState.prototype.clear = function () {
    this.ctx.clearRect(0, 0, this.width, this.height);
}

CanvasState.prototype.clearSelection = function () {
    for (var i = 0; i < this.maxRows; i++) {
        for (var j = 0; j < this.maxCols; j++) {
            var shape = this.shapes[i][j];
            shape.selected = false;
        }
    }
    this.selection = false;
    this.mousePressed = false;
    this.valid = false;
}

CanvasState.prototype.setSelectionTo = function (newState) {
    for (var i = 0; i < this.maxRows; i++) {
        for (var j = 0; j < this.maxCols; j++) {
            var shape = this.shapes[i][j];
            if (shape.selected) {
                shape.state = newState;
            }
        }
    }
    this.valid = false;
}


CanvasState.prototype.showInfo = function () {
    var totalBlue = 0;
    var totalGray = 0;
    var totalRows = 0;
    var existsRow = false;
    for (var i = 0; i < this.maxRows; i++) {
        existsRow = false;
        for (var j = 0; j < this.maxCols; j++) {
            var shape = this.shapes[i][j];
            if (shape.state) {
                totalBlue = totalBlue + 1;
                existsRow = true;
            } else {
                totalGray = totalGray + 1;
            }
        }
        if (existsRow) {
            totalRows = totalRows + 1;
        }
    }

    document.getElementById('labelBlue').innerHTML = totalBlue;
    document.getElementById('labelGray').innerHTML = totalGray;
    document.getElementById('labelRows').innerHTML = totalRows;

}

// While draw is called as often as the INTERVAL variable demands,
// It only ever does something if the canvas gets invalidated by our code
CanvasState.prototype.draw = function () {
    // if our state is invalid, redraw and validate!
    if (!this.valid) {
        this.clear();
        var ctx = this.ctx;
        var shapes = this.shapes;

        //border
        ctx.strokeStyle = '#000000';
        ctx.lineWidth = 1;
        ctx.strokeRect(0, 0, ctx.canvas.width, ctx.canvas.height);

        // ** Add stuff you want drawn in the background all the time here **

        // draw all shapes
        for (var i = 0; i < this.maxRows; i++) {
            for (var j = 0; j < this.maxCols; j++) {
                var shape = shapes[i][j];
                // We can skip the drawing of elements that have moved off the screen:
                //if (shape.x > this.width || shape.y > this.height ||
                //shape.x + shape.w < 0 || shape.y + shape.h < 0) continue;
                this.drawShape(i, j, shape);
            }
        }

        // draw selection
        // right now this is just a stroke along the edge of the selected Shape
         if (this.selection) {
             ctx.strokeStyle = this.selectionColor;
             ctx.lineWidth = this.selectionWidth;
             ctx.strokeRect(this.initialX, this.initialY, this.selectionX - this.initialX, this.selectionY - this.initialY);
         }

        // ** Add stuff you want drawn on top all the time here **

        this.valid = true;
    }
}


// Draws this shape to a given context
CanvasState.prototype.drawShape = function (row, col, shape) {

    var x = col * this.itemWidth;
    var y = row * this.itemHeight;

    if (shape.selected) {
        if (shape.state) {
            this.ctx.fillStyle = '#CCCC00';
        } else {
            this.ctx.fillStyle = '#EEEE00';
        }

    }
    else {
        if (shape.state) {
            this.ctx.fillStyle = '#0000AA';
        } else {
            this.ctx.fillStyle = '#CCCCCC';
        }
    }

    this.ctx.fillRect(x, y, this.itemWidth, this.itemHeight);

    this.ctx.beginPath();
    this.ctx.moveTo(x + this.itemWidth, y);
    this.ctx.lineTo(x, y);
    this.ctx.lineTo(x, y + this.itemHeight);
    this.ctx.lineWidth = 1;
    this.ctx.strokeStyle = '#FFFFFF';
    this.ctx.stroke();

    this.ctx.beginPath();
    this.ctx.moveTo(x, y + this.itemHeight);
    this.ctx.lineTo(x + this.itemWidth, y + this.itemHeight);
    this.ctx.lineTo(x + this.itemWidth, y);
    this.ctx.lineWidth = 1;
    this.ctx.strokeStyle = '#000000';
    this.ctx.stroke();
}


CanvasState.prototype.checkIntersection = function (shape) {

    var shapeX = shape.col * this.itemWidth;
    var shapeY = shape.row * this.itemHeight;
    var selX = this.initialX < this.selectionX ? this.initialX : this.selectionX;
    var selY = this.initialY < this.selectionY ? this.initialY : this.selectionY;

    var selWidth = Math.abs(this.selectionX - this.initialX);
    var selHeight = Math.abs(this.selectionY - this.initialY);

    return !(selX + selWidth < shapeX ||
           shapeX + this.itemWidth < selX ||
           selY + selHeight < shapeY ||
           shapeY + this.itemHeight < selY);

    //     return !(rectA.x + rectA.width < rectB.x ||
    //            rectB.x + rectB.width < rectA.x ||
    //            rectA.y + rectA.height < rectB.y ||
    //            rectB.y + rectB.height < rectA.y);
}; 

// Creates an object with x and y defined, set to the mouse position relative to the state's canvas
// If you wanna be super-correct this can be tricky, we have to worry about padding and borders
CanvasState.prototype.getMouse = function (e) {
    var element = this.canvas, offsetX = 0, offsetY = 0, mx, my;

    // Compute the total offset
    if (element.offsetParent !== undefined) {
        do {
            offsetX += element.offsetLeft;
            offsetY += element.offsetTop;
        } while ((element = element.offsetParent));
    }

    // Add padding and border style widths to offset
    // Also add the <html> offsets in case there's a position:fixed bar
    offsetX += this.stylePaddingLeft + this.styleBorderLeft + this.htmlLeft;
    offsetY += this.stylePaddingTop + this.styleBorderTop + this.htmlTop;

    mx = e.pageX - offsetX;
    my = e.pageY - offsetY;

    // We return a simple javascript object (a hash) with x and y defined
    return { x: mx, y: my };
}

CanvasState.prototype.getItem = function (mx, my) {
    mx = mx - this.offsetX;
    my = my - this.offsetY;
    var r = parseInt(my / this.itemHeight);
    var c = parseInt(mx / this.itemWidth);
    return { col: c, row: r };
}

// If you dont want to use <body onLoad='init()'>
// You could uncomment this init() reference and place the script reference inside the body tag
//init();

function init() {
    
    //настройка ширины канвы
    var canvasParent = document.getElementById('canvasParent');
    var s = new CanvasState(document.getElementById('canvas'), canvasParent.clientWidth - 10, canvasParent.clientHeight);

    for (var i = 0; i < 15; i++) {
        for (var j = 0; j < 10; j++) {
            s.addShape(i, j, true); // The default is gray
        } 
    }
    s.showInfo();

    var setState1 = document.getElementById('setState1');
    setState1.onclick = function (e) {
        s.setSelectionTo(true);
        s.clearSelection();
        s.showInfo();
    };

    var setState0 = document.getElementById('setState0');
    setState0.onclick = function (e) {
        s.setSelectionTo(false); 
        s.clearSelection();
        s.showInfo();
    };

    var clearSelectionBtn = document.getElementById('clearSelection');
    clearSelectionBtn.onclick = function (e) {
        s.clearSelection();
    };


    //s.addShape(new Shape(50, 10, 30, 30, true, true));
    // Lets make some partially transparent
    //s.addShape(new Shape(90, 10, 30, 30, true, false));
    //s.addShape(new Shape(130, 10, 30, 30, true));
}

// Now go make something amazing!

