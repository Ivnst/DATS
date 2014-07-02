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
    this.row = row || 0;
    this.col = col || 0;
    this.state = state || 0;
    this.selected = selected || 0;
    this.label = 0;
}

function CanvasState(canvas, width, height) {
    // **** First some setup! ****

    this.itemWidth = 25;  //ширина клетки (может изменяться при изменении масштаба)
    this.itemHeight = 25; //высота клетки
    this.maxRows = 25;    //общее количество строк (серых)
    this.maxCols = 50;    //общее количество стролбцов (серых)
    this.offsetX = 0;     //сдвиг по горизонтали (для автоскроллинга)
    this.offsetY = 0;     //сдвиг по вертикали (для автоскроллинга)

    this.canvas = canvas;
    //настройка ширины канвы
    this.canvas.width = width;
    this.canvas.height = height;

    this.ctx = canvas.getContext('2d');
    this.ctx.textAlign = "center";
    this.ctx.textBaseline = "middle";

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

    // the collection of things to be drawn
    this.shapes = null;

    this.reloadData();

    this.valid = false;     // when set to false, the canvas will redraw everything
    this.selection = false; //
    this.initialX = 0;      // начальное положение указателя мыши при выделении
    this.initialY = 0;
    this.selectionX = 0;    //текущее положение указателя мыши во время выделения
    this.selectionY = 0;
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

    // Up, down, and move
    canvas.addEventListener('mousedown', function (e) {
        var mouse = myState.getMouse(e);
        myState.initialX = mouse.x + myState.offsetX;
        myState.initialY = mouse.y + myState.offsetY;
        myState.mousePressed = true;
        myState.firstSelectedItem = myState.getItem(myState.initialX, myState.initialY);

    }, true);

    canvas.addEventListener('mousemove', function (e) {

        //определяем координаты курсора мыши
        var mouse = myState.getMouse(e);

        myState.adjustPosition(mouse.x, mouse.y);

        //---------------------------------------------
        //if mouse button is not pressed - return
        if (!myState.mousePressed) return;

        myState.selectionX = mouse.x + myState.offsetX;
        myState.selectionY = mouse.y + myState.offsetY;
        var itm = myState.getItem(myState.selectionX, myState.selectionY);
        if (itm.row == myState.firstSelectedItem.row && itm.col == myState.firstSelectedItem.col && !myState.selection) return;
        myState.selection = true;

        //set selection
        var selectionCount = 0;
        if (myState.shapes != null)
            for (var i = 0; i < myState.maxRows; i++) {
                for (var j = 0; j < myState.maxCols; j++) {
                    var shape = myState.shapes[i][j];
                    if (shape.state != -1)
                        shape.selected = myState.checkIntersection(shape);
                    if (shape.selected) selectionCount += 1;
                }
            }

        document.getElementById('lblCountSelected').innerHTML = selectionCount;

        myState.valid = false; // must redraw
    }, true);

    canvas.addEventListener('mouseup', function (e) {
        if (!myState.mousePressed) return;
        if (!myState.selection) {
            // myState.clearSelection();
            var mouse = myState.getMouse(e);
            mouse.x += myState.offsetX;
            mouse.y += myState.offsetY;
            var itm = myState.getItem(mouse.x, mouse.y);

            if (myState.shapes != null) {
                if (itm.col >= 0 && itm.row >= 0 && itm.col < myState.maxCols && itm.row < myState.maxRows) {
                    //change selection
                    var shape = myState.shapes[itm.row][itm.col];
                    if (shape != undefined)
                        shape.selected = !shape.selected;
                }
                else {
                    myState.clearSelection();
                } 
            }
        }
        myState.selection = false;
        myState.mousePressed = false;
        myState.firstSelectedItem = null;
        myState.valid = false; // Need to clear the old selection border

    }, true);

    // double click
    canvas.addEventListener('dblclick', function (e) {
    }, true);


    //refresh interval!
    this.interval = 30;
    setInterval(function () { myState.draw(); }, myState.interval);
}

//Add new shape on canvas with specified position and state
CanvasState.prototype.reloadData = function () {

    var myState = this;

    //get sector places data
    $.get("/Sector/SectorSoldInfo?sid=" + params.sid + "&mid=" + params.mid,
    function (data) {
        myState.refresh(data);
    })
}

//Add new shape on canvas with specified position and state
CanvasState.prototype.refresh = function (data) {

    var minRow = 100, maxRow = 0, minCol = 100, maxCol = 0;

    //узнаём размерность сектора
    for (var i = 0; i < data.length; i++) {
        var itm = data[i];
        if (itm.Row < minRow) minRow = itm.Row;
        if (itm.Row > maxRow) maxRow = itm.Row;
        if (itm.Col < minCol) minCol = itm.Col;
        if (itm.Col > maxCol) maxCol = itm.Col;
    }

    //определяем ширину и высоту сектора
    var width = maxCol - minCol + 1;
    var height = maxRow - minRow + 1;

    //создаём новую матрицу
    var newShapes = [];
    for (var i = 0; i < height; i++) {
        newShapes[i] = [];
    }
    for (var i = 0; i < height; i++) {
        for (var j = 0; j < width; j++) {
            newShapes[i][j] = new Shape(i, j, -1, false);
        }
    }

    //заполняем матрицу данными
    for (var i = 0; i < data.length; i++) {
        var itm = data[i];
        newShapes[itm.Row - minRow][itm.Col - minCol].state = itm.State;
        newShapes[itm.Row - minRow][itm.Col - minCol].label = itm.Num;
    }

    this.shapes = newShapes;
    this.maxCols = width;
    this.maxRows = height;
    this.adjustPosition(0, 0);
    this.valid = false;
    this.showInfo();
}

//Add new shape on canvas with specified position and state
CanvasState.prototype.addShape = function (row, col, state) {
    var shape = this.shapes[row][col];
    shape.state = state;
    this.valid = false;
}

//Adjust places position
CanvasState.prototype.adjustPosition = function (mx, my) {
    //autoscrolling
    if (this.itemWidth * this.maxCols > this.canvas.width) {
        var sizeDiffX = this.itemWidth * (this.maxCols + 2) - this.canvas.width;

        //var percentX = mouse.x * 100 / myState.canvas.width;

        this.offsetX = parseInt(sizeDiffX * mx / this.canvas.width) - this.itemWidth;
    } else {
        var sizeDiffX = this.canvas.width - this.itemWidth * this.maxCols;
        this.offsetX = -parseInt(sizeDiffX / 2);
    }


    if (this.itemHeight * this.maxRows > this.canvas.height) {
        var sizeDiffY = this.itemHeight * (this.maxRows + 2) - this.canvas.height;

        //var percentY = mouse.y * 100 / myState.canvas.height;

        this.offsetY = parseInt(sizeDiffY * my / this.canvas.height) - this.itemHeight;
    } else {
        var sizeDiffY = this.canvas.height - this.itemHeight * this.maxRows;
        this.offsetY = -parseInt(sizeDiffY / 2);
    }
    this.valid = false;
}

//Clear all canvas
CanvasState.prototype.clear = function () {
    this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
}

//Clear all selection
CanvasState.prototype.clearSelection = function () {
    if (this.shapes != null)
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

//changes selected cell's state to specified value
CanvasState.prototype.setSelectionTo = function (newState) {
    var num = 1;
    for (var i = 0; i < this.maxRows; i++) {
        num = 1;
        for (var j = 0; j < this.maxCols; j++) {
            var shape = this.shapes[i][j];
            if (shape.selected) {
                shape.state = newState;
            }
            if (shape.state) {
                shape.label = num;
                num = num + 1;
            }
        }
    }
    this.valid = false;
}

//calculates total cells count
CanvasState.prototype.showInfo = function () {
    var totalPlaces = 0;
    var totalFree = 0;
    var totalSold = 0;
    var totalReserved = 0;

    for (var i = 0; i < this.maxRows; i++) {
        for (var j = 0; j < this.maxCols; j++) {
            var shape = this.shapes[i][j];
            if (shape.state == -1) continue;
            totalPlaces += 1;
            if (shape.state == 0) { totalFree += 1; }
            if (shape.state == 1) { totalSold += 1; }
            if (shape.state == 2) { totalReserved += 1; }
        }
    }

    document.getElementById('lblCountAll').innerHTML = totalPlaces;
    document.getElementById('lblCountFree').innerHTML = totalFree;
    document.getElementById('lblCountSold').innerHTML = totalSold;
    document.getElementById('lblCountReserved').innerHTML = totalReserved;
    document.getElementById('lblPrice').innerHTML = 0;

}

//change item size on specified value
CanvasState.prototype.scale = function (val) {
    this.itemWidth = this.itemWidth + val;
    this.itemHeight = this.itemHeight + val;
    this.adjustPosition(0, 0);
    this.valid = false;
}

// While draw is called as often as the INTERVAL variable demands,
// It only ever does something if the canvas gets invalidated by our code
CanvasState.prototype.draw = function () {
    // if our state is invalid, redraw and validate!
    if (!this.valid) {
        this.valid = true;
        this.clear();
        var ctx = this.ctx;
        var shapes = this.shapes;
        if (shapes == null) return;

        //border
        ctx.save();
        ctx.strokeStyle = '#000000';
        ctx.lineWidth = 1;
        ctx.strokeRect(0, 0, ctx.canvas.width, ctx.canvas.height);

        // draw all shapes
        for (var i = 0; i < this.maxRows; i++) {
            for (var j = 0; j < this.maxCols; j++) {
                this.drawShape(i, j, shapes[i][j]);
            }
        }

        //rows numbers
        for (var i = 0; i < this.maxRows; i++) {

            var y = i * this.itemHeight - this.offsetY;

            this.ctx.font = "12pt Arial";
            this.ctx.lineWidth = 2;
            this.ctx.strokeStyle = 'black';
            this.ctx.strokeText(i + 1, this.itemWidth / 2, y + this.itemHeight / 2);
            ctx.fillStyle = 'white';
            this.ctx.fillText(i + 1, this.itemWidth / 2, y + this.itemHeight / 2);
        }


        // draw selection
        if (this.selection) {
            ctx.strokeStyle = '#CC0000';
            ctx.lineWidth = 2;
            ctx.globalAlpha = 0.6;
            ctx.strokeRect(this.initialX - this.offsetX, this.initialY - this.offsetY, this.selectionX - this.initialX, this.selectionY - this.initialY);
            ctx.globalAlpha = 1;
        }
        ctx.restore();
    }
}


// Draws this shape to a given context
CanvasState.prototype.drawShape = function (row, col, shape) {

    if (shape.state == -1) return;

    var x = col * this.itemWidth - this.offsetX;
    var y = row * this.itemHeight - this.offsetY;

    if (x < -this.itemWidth || y < -this.itemHeight) return;
    if (x > this.canvas.width || y > this.canvas.height) return;

    //shape color
    if (shape.state == 0) { this.ctx.fillStyle = (shape.selected) ? '#CCCC00' : '#00AA00'; }
    if (shape.state == 1) { this.ctx.fillStyle = (shape.selected) ? '#CCCC00' : '#AA0000'; }
    if (shape.state == 2) { this.ctx.fillStyle = (shape.selected) ? '#CCCC00' : '#0000AA'; }

    this.ctx.fillRect(x, y, this.itemWidth, this.itemHeight);

    //border line
    this.ctx.beginPath();
    this.ctx.moveTo(x, y + this.itemHeight);
    this.ctx.lineTo(x + this.itemWidth, y + this.itemHeight);
    this.ctx.lineTo(x + this.itemWidth, y);
    this.ctx.lineWidth = 1;
    this.ctx.strokeStyle = '#000000';
    this.ctx.stroke();

    //border line
    this.ctx.beginPath();
    this.ctx.moveTo(x + this.itemWidth, y);
    this.ctx.lineTo(x, y);
    this.ctx.lineTo(x, y + this.itemHeight);
    this.ctx.lineWidth = 1;
    this.ctx.strokeStyle = '#FFFFFF';
    this.ctx.stroke();

    //label (place number)
    if (shape.state != -1) {
        this.ctx.fillStyle = '#FFFFFF';
        this.ctx.font = "10pt Courier New";
        this.ctx.fillText(shape.label, x + this.itemWidth / 2, y + this.itemHeight / 2);
    }
}

//check item intersection with selection rectangle
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

//gets item coordinates accroding to mouse coordinates
CanvasState.prototype.getItem = function (mx, my) {
    var signY = 1, signX = 1;
    if (mx < 0) { signX = -1; mx = -mx; }
    if (my < 0) { signY = -1; my = -my; }

    var r = parseInt(my / this.itemHeight) * signY;
    var c = parseInt(mx / this.itemWidth) * signX;
    return { col: c, row: r };
}

//Sends data to server
CanvasState.prototype.sendData = function (newState) {

    var sectors = [];

    for (var i = 0; i < this.maxRows; i++) {
        for (var j = 0; j < this.maxCols; j++) {
            var shape = this.shapes[i][j];
            if (shape.selected) {
                var itm = new Object();
                itm.Row = shape.row;
                itm.Col = shape.col;
                itm.Num = shape.label;
                itm.State = newState;

                sectors.push(itm);
            }
        }
    }
    var resultString = JSON.stringify(sectors);
    var myState = this;

    $.post("/Sector/StoreSectorSoldInfo", { sid: params.sid, mid: params.mid, data: resultString },
    function (data) {

        myState.reloadData();

        data = data.split("<!")[0];
        alert(data);
    })
}

// If you dont want to use <body onLoad='init()'>
// You could uncomment this init() reference and place the script reference inside the body tag
//init();

var params = {};

function init() {

    //get params from url
    if (location.search) {
        var parts = location.search.substring(1).split('&');

        for (var i = 0; i < parts.length; i++) {
            var nv = parts[i].split('=');
            if (!nv[0]) continue;
            params[nv[0]] = nv[1] || true;
        }
    }

    //create canvas
    var canvasParent = document.getElementById('canvasParent');
    var s = new CanvasState(document.getElementById('canvas'), canvasParent.clientWidth - 10, canvasParent.clientHeight);

    //set buttons functionality

    document.getElementById('btnSell').onclick = function (e) {
        s.sendData(1);
        s.clearSelection();
    };

    document.getElementById('btnReturn').onclick = function (e) {
        s.sendData(0);
        s.clearSelection();
    };

    document.getElementById('btnReserve').onclick = function (e) {
        //         s.setSelectionTo(false); 
        //         s.clearSelection();
        //         s.showInfo();
    };

    document.getElementById('clearSelection').onclick = function (e) {
        s.clearSelection();
    };

    document.getElementById('btnScalePlus').onclick = function (e) {
            s.scale(5);
    };

    document.getElementById('btnScaleMinus').onclick = function (e) {
        if (s.itemWidth > 10)
            s.scale(-5);
    };
}


/*
 *	$.get("my/uri", 
    function(data) { 
       // client side logic using the data from the server
    })

Sending data from javascript to the server works similarly:

$.post("my/uri", { aValue : "put any data for the server here" }
    function(data) { 
       // client side logic using the data returned from the server
    })
 */