var labelColor = [
    { "color": [120, 78, 129] },    // violet
    { "color": [64, 67, 153] },     // indigo
    { "color": [72, 139, 194] },    // blue
    { "color": [107, 178, 140] },   // green
    { "color": [159, 190, 87] },    // olive
    { "color": [210, 179, 63] },    // yellow
    { "color": [231, 126, 49] },    // orange
    { "color": [218, 33, 32] },     // red
    { "color": [81, 205, 213] }     // cyan

];

var dictColor = {};


function assignColorToLabel(labels) {
    for (var index = 0; index < labels.length; index++) {
        dictColor[labels[index]] = labelColor[index].color;
    }
}

function mover(d) {
    var el = d3.select(this)
            .transition()
            .duration(10)
            .style("fill-opacity", 0.3)
        ;

}

//Mouseout function
function mout(d) {
    var el = d3.select(this)
            .transition()
            .duration(1000)
            .style("fill-opacity", 1)
        ;
}

function rgb(array) {
    return "rgb(" + array.map(function (r) { return Math.round(r); }).join(",") + ")";
}

function setColor(label) {
    var color = labelColor[label];
    return "rgb(${color})";
}

function flatten(arr) {
    var flattenNodes = [];

    const row = arr.length;
    const col = arr[0].length;

    for (i = 0; i < col; i++) {
        for (j = 0; j < row; j++) {
            flattenNodes.push(arr[i][j]);
        }
    }

    return flattenNodes;
}

function readCsvLine(line) {
    return line.split(",");
}

var tooltip = d3.select("body")
    .append("div")
    .style("position", "absolute")
    .style("z-index", "10")
    .style("visibility", "hidden");

$('#regions').click(function () {
    $('#modal1').openModal();
});

$('#addRegionButton').click(function () {
    $('#modal1').closeModal();
});