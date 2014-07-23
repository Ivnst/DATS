var form = 2;

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
        if (itm.rowPos == myState.firstSelectedItem.rowPos && itm.colPos == myState.firstSelectedItem.colPos && !myState.selection) return;
        myState.selection = true;

        //set selection
        var selectionCount = 0;
        var selectionSumm = 0;
        if (myState.shapes != null)
            for (var i = 0; i < myState.maxRows; i++) {
                for (var j = 0; j < myState.maxCols; j++) {
                    var shape = myState.shapes[i][j];
                    if (shape.state != -1)
                        shape.selected = myState.checkIntersection(j, i);
                    if (shape.selected) {
                        selectionCount += 1;
                        selectionSumm += shape.price;
                    }
                }
            }

        document.getElementById('lblCountSelected').innerHTML = selectionCount;
        document.getElementById('lblTotalSumm').innerHTML = selectionSumm;

        myState.valid = false; // must redraw
        myState.RefreshButtons();
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
                if (itm.colPos >= 0 && itm.rowPos >= 0 && itm.colPos < myState.maxCols && itm.rowPos < myState.maxRows) {
                    //change selection
                    var shape = myState.shapes[itm.rowPos][itm.colPos];
                    if (shape != null && shape.state != -1)
                        shape.selected = !shape.selected;
                    else
                        myState.clearSelection();
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
        myState.RefreshButtons();
        myState.showInfo();

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

    var maxRow = 0, maxCol = 0;
    var minRow = 100, minCol = 100;

    //узнаём размерность сектора
    for (var i = 0; i < data.length; i++) {
        var itm = data[i];
        if (itm.RowPos > maxRow) maxRow = itm.RowPos;
        if (itm.ColPos > maxCol) maxCol = itm.ColPos;
        if (itm.RowPos < minRow) minRow = itm.RowPos;
        if (itm.ColPos < minCol) minCol = itm.ColPos;
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
        var shape = newShapes[itm.RowPos - minRow][itm.ColPos - minCol];
        shape.state = itm.State;
        shape.row = itm.Row;
        shape.col = itm.Col;
        shape.price = itm.Price;
    }

    this.shapes = newShapes;
    this.maxCols = width;
    this.maxRows = height;
    this.adjustPosition(0, 0);
    this.valid = false;
    this.showInfo();
    this.RefreshButtons();
}


//calculates total cells count
CanvasState.prototype.showInfo = function () {
    var totalPlaces = 0;
    var totalFree = 0;
    var totalSold = 0;
    var totalReserved = 0;
    var totalSelected = 0;
    var totalSumm = 0;

    for (var i = 0; i < this.maxRows; i++) {
        for (var j = 0; j < this.maxCols; j++) {
            var shape = this.shapes[i][j];
            if (shape.selected) {
                totalSelected++;
                totalSumm += shape.price;
            }
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
    document.getElementById('lblPrice').innerHTML = currPrice;
    document.getElementById('lblCountSelected').innerHTML = totalSelected;
    document.getElementById('lblTotalSumm').innerHTML = totalSumm;

}


CanvasState.prototype.getShapeColor = function (shape) {
    //shape color
    if (shape.state == 0) { return (shape.selected) ? '#CCCC00' : '#00AA00'; } //free
    if (shape.state == 1) { return (shape.selected) ? '#CCAA00' : '#AA0000'; } //sold
    if (shape.state == 2) { return (shape.selected) ? '#CCCCAA' : '#0000AA'; } //reserved
    return '000000';
}


//Sends data to server
CanvasState.prototype.sendData = function (newState) {

    var sectors = this.getSelectedItemsList();

    //если ничего не выбрали, то сообщаем об этом пользователю и ничего не делаем
    if (sectors.length == 0) {
        $('#myModal').modal({
            remote: '/Utils/MessageBox?header=' + encodeURIComponent('Внимание!') + '&message=' + encodeURIComponent('Не выбраны места для осуществления операции!')
        })
        return;
    }

    //присваиваем новый статус (он будет отображать действие, которое мы хотим выполнить)
    for (var i = 0; i < sectors.length; i++) {
        sectors[i].State = newState;
    }

    //кодируем места в json
    var resultString = JSON.stringify(sectors);
    var myState = this;

    //this.RefreshButtons();

    $.post("/Utils/CacheData", { data: resultString },
    function (data) {
        //здесь data - это ключ, полученный от метода Utils/CacheData
        data = data.split("<!")[0];

        $('#myModal').modal({
            remote: '/Sector/Confirm?sid=' + params.sid + '&mid=' + params.mid + '&dataKey=' + data
        })

    })
}


//Sends data to server
CanvasState.prototype.sendDataForReservation = function () {

    var sectors = this.getSelectedItemsList();

    //если ничего не выбрали, то сообщаем об этом пользователю и ничего не делаем
    if (sectors.length == 0) {
        ShowMessageBox('Не выбраны места для осуществления операции!', 'Внимание!', true);
//         $('#myModal').modal({
//             remote: '/Utils/MessageBox?header=' + encodeURIComponent('Внимание!') + '&message=' + encodeURIComponent('Не выбраны места для осуществления операции!')
//         })
        return;
    }

    //кодируем места в json
    var resultString = JSON.stringify(sectors);
    var myState = this;

    $.post("/Utils/CacheData", { data: resultString },
    function (data) {
        //здесь data - это ключ, полученный от метода Utils/CacheData
        data = data.split("<!")[0];

        
        $('#myModal').modal({
            remote: '/Reservation/Create?sid=' + params.sid + '&mid=' + params.mid + '&data=' + data
        })

    })

}


//Returns selected items
CanvasState.prototype.getSelectedItemsList = function (newState) {

    var sectors = [];

    //collect selected shapes
    for (var i = 0; i < this.maxRows; i++) {
        for (var j = 0; j < this.maxCols; j++) {
            var shape = this.shapes[i][j];
            if (shape.selected) {
                var itm = new Object();
                itm.Row = shape.row;
                itm.Col = shape.col;
                itm.RowPos = i;
                itm.ColPos = j;
                itm.State = shape.state;
                itm.Price = shape.price;

                sectors.push(itm);
            }
        }
    }
    return sectors;
}

//Disables sell and return buttons
CanvasState.prototype.RefreshButtons = function () {

    var showSell = true;
    var showReturn = true;
    var showReserve = true;
    var lastState = -1;
    var selectedCount = 0;

    for (var i = 0; i < this.maxRows; i++) {
        for (var j = 0; j < this.maxCols; j++) {
            var shape = this.shapes[i][j];
            if (shape.selected) {

                //если выделены ячейки с разным статусом
                if (lastState != -1 && lastState != shape.state) {
                    showSell = showReturn = showReserve = false;
                    continue;
                }

                if (shape.state == 1) { showSell = showReserve = false; }
                if (shape.state == 0) showReturn = false;
                if (shape.state == 2) showReserve = false;
                lastState = shape.state;
                selectedCount++;
            }
        }
    }

    if (selectedCount == 0) {
        showSell = showReturn = showReserve = false;
    }

    $('#btnSell').prop("disabled", !showSell);
    $('#btnReturn').prop("disabled", !showReturn);
    $('#btnReserve').prop("disabled", !showReserve);
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
    document.getElementById('btnSell').onclick = function (e) { s.sendData(1); };
    document.getElementById('btnReturn').onclick = function (e) { s.sendData(0); };
    document.getElementById('btnReserve').onclick = function (e) { s.sendDataForReservation(); };
    document.getElementById('btnScalePlus').onclick = function (e) { if (s.itemWidth < 100) s.scale(5); };
    document.getElementById('btnScaleMinus').onclick = function (e) { if (s.itemWidth > 15) s.scale(-5); };
    document.getElementById('clearSelection').onclick = function (e) { s.reloadData(); s.clearSelection(); };
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