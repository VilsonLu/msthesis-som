function initMap(map) {
    nodes = map;
    x = 20;
    y = 20;
}


var nodes = [];
var n = 20;
var sen = 20;

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
};

    

    
