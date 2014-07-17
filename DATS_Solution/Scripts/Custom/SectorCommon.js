// Constructor for Shape objects to hold data for all drawn objects.
function Shape(row, col, state, selected) {
    // This is a very simple and unsafe constructor. All we're doing is checking if the values exist.
    // "x || 0" just means "if there is a value for x, use that. Otherwise use 0."
    // But we aren't checking anything else! We could put "Lalala" for the value of x 
    this.row = row || 0;
    this.col = col || 0;
    this.state = state || 0;
    this.selected = selected || 0;
    this.price = 0;
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
            var hasSelected = false;
            for (var j = 0; j < this.maxCols; j++) {
                this.drawShape(i, j, shapes[i][j]);
                if (shapes[i][j].selected) hasSelected = true;
            }
            //rows numbers
            this.drawRowNumber(i, this.maxRows - i, hasSelected);
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

    if (form == 2 && shape.state == -1) return;

    var x = col * this.itemWidth - this.offsetX;
    var y = row * this.itemHeight - this.offsetY;

    if (x < -this.itemWidth || y < -this.itemHeight) return;
    if (x > this.canvas.width || y > this.canvas.height) return;

    //shape color
    this.ctx.fillStyle = this.getShapeColor(shape);
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
    if ((shape.state && form == 1) || (shape.state != -1 && form == 2)) {
        this.ctx.fillStyle = '#FFFFFF';
        var fontSize = parseInt(this.itemWidth / 5 + 9);
        this.ctx.font = fontSize + "pt Sans Serif";
        this.ctx.fillText(shape.col, x + this.itemWidth / 2, y + this.itemHeight / 2);
    }
}


// Draws row number on left side of the canvas
CanvasState.prototype.drawRowNumber = function (row, val, highlight) {
    var y = row * this.itemHeight - this.offsetY;
    var x = -this.offsetX - this.itemWidth;
    if (x < this.itemWidth / 2) x = this.itemWidth / 2;
    var fontSize = parseInt(this.itemWidth / 5 + 11).toString();
    this.ctx.font = (highlight ? "bold " : " ") + fontSize + "pt Arial";
    this.ctx.lineWidth = 2;
    this.ctx.strokeStyle = 'black';
    this.ctx.strokeText(val, x, y + this.itemHeight / 2);

    if (highlight) {
        this.ctx.fillStyle = 'DarkOrange';
    } else {
        this.ctx.fillStyle = 'white';
    }

    this.ctx.fillText(val, x, y + this.itemHeight / 2);
}


//check item intersection with selection rectangle
CanvasState.prototype.checkIntersection = function (x, y) {

    var shapeX = x * this.itemWidth;
    var shapeY = y * this.itemHeight;
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
    return { colPos: c, rowPos: r };
}