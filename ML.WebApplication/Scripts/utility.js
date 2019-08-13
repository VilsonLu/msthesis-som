var labelColor = [  
    { "color": [255, 189, 114] },   // orange
    { "color": [81, 205, 213] },    // cyan
    { "color": [107, 178, 140] },   // green
    { "color": [210, 179, 63] },    // yellow
    { "color": [64, 67, 153] },     // indigo
    { "color": [72, 139, 194] },    // blue
    { "color": [159, 190, 87] },    // olive
    { "color": [231, 126, 49] },    // orange
    { "color": [218, 33, 32] },     // red
    { "color": [120, 78, 129] },    // violet
    { "color": [23, 48, 34] },      // amazon
    { "color": [60, 40, 80] },      // amethyst
    { "color": [65, 16, 16] },      // auburn
    { "color": [54, 81, 94] },      // baby blue
    { "color": [80, 50, 20] },      // bronze
    { "color": [11, 30, 24] },      // brunswick green
    { "color": [80, 33, 0] },       // burnt orange
    { "color": [0, 80, 60] },       // carribean green
    { "color": [89, 26, 20] },      // cinnabar
    { "color": [82, 41, 12] },      // cocoa brown
    { "color": [100, 74, 85] }      // cotton candy

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