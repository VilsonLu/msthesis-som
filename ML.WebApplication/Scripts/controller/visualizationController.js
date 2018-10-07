app.controller("visualizationController",
    function ($scope) {

        $scope.Data = {};
        $scope.ShowLoader = false;
        var nodes = [];
        var n = 20;
        var sen = 50;


        function getLabels(nodes) {
            return nodes.map(item => item.Label)
                .filter((value, index, self) => self.indexOf(value) === index);
        }

        function getClusters(nodes) {
            return nodes.map(item => item.ClusterGroup)
                .filter((value, index, self) => self.indexOf(value) === index);
        }

        var visualizeSOM = function () {

            d3.select("svg").remove();

            var w = $scope.Data.Width * sen;
            var h = $scope.Data.Height * sen;
            var svg = d3.select("#chart").append("svg").attr("width", w).attr("height", h),
                width = w,
                height = h
                ;



            var rgb_nodes = svg.append("g").attr("class", "nodes all");

            var nodes = flatten($scope.Data.Map);
            var labels = getLabels(nodes);

            assignColorToLabel(labels);

            rgb_nodes
                .selectAll("rect")
                .data(nodes)
                .enter().append("rect")
                .attr("y", function (node) { return node.Coordinate.X * sen; })
                .attr("x", function (node) { return node.Coordinate.Y * sen; })
                .attr("width", sen)
                .attr("height", sen)
                .attr("text", function (node) { return node.Label })
                .style("fill",
                function (node) {
                    return rgb(dictColor[node.Label]);
                })
                .on("mouseover",
                function (node) {
                    mover();
                    return tooltip.style("visibility", "visible")
                        .text(node.Label);
                })
                .on("mousemove",
                function () {
                    return tooltip.style("top", (event.pageY - 10) + "px").style("left", (event.pageX + 10) + "px");
                })
                .on("mouseout",
                function (node) {
                    mout();
                    return tooltip.style("visibility", "hidden");
                });

            var lineData = [{ "x": 1, "y": 1 }
                , { "x": 2, "y": 2 }
                , { "x": 3, "y": 3 }
                , { "x": 4, "y": 4 }
                , { "x": 5, "y": 5 }
                , { "x": 6, "y": 6 }
                , { "x": 7, "y": 7 }
                , { "x": 8, "y": 8 }
                , { "x": 9, "y": 9 }
                , { "x": 0, "y": 0 }

                , { "x": 1, "y": 2 }];

            var coordinateMapper = function (d) {
                return d * sen + 0.5 * sen
            }

            svg.selectAll("circle")
                .data(lineData).enter()
                .append("circle")
                .attr("cy", function (d) { return coordinateMapper(d["x"]); })
                .attr("cx", function (d) { return coordinateMapper(d["y"]); })
                .attr("r", "6px")
                .attr("fill", "red")

        };

        var visualizeCluster = function() {
            d3.select("svg").remove();
            var w = $scope.Data.Width * sen;
            var h = $scope.Data.Height * sen;
            var svg = d3.select("#chart").append("svg").attr("width", w).attr("height", h),
                width = w,
                height = h;

            var rgb_nodes = svg.append("g").attr("class", "nodes all");

            var nodes = flatten($scope.Data.Map);
            var labels = getClusters(nodes);

            assignColorToLabel(labels);

            rgb_nodes
                .selectAll("rect")
                .data(nodes)
                .enter().append("rect")
                .attr("x", function (node) { return node.Coordinate.X * sen; })
                .attr("y", function (node) { return node.Coordinate.Y * sen; })
                .attr("width", sen)
                .attr("height", sen)
                .attr("text", function (node) { return node.Label })
                .style("fill",
                    function (node) {
                        return rgb(dictColor[node.ClusterGroup]);
                    })
                .on("mouseover",
                    function (node) {
                        mover();
                        return tooltip.style("visibility", "visible")
                            .text(node.Label);
                    })
                .on("mousemove",
                    function () {
                        return tooltip.style("top", (event.pageY - 10) + "px")
                            .style("left", (event.pageX + 10) + "px");
                    })
                .on("mouseout",
                    function (node) {
                        mout();
                        return tooltip.style("visibility", "hidden");
                    });
        }


        // Shows the cluster group
        $scope.isShowCluster = false;
        $scope.showCluster = function() {
            if ($scope.isShowCluster) {
                visualizeCluster();
            } else {
                visualizeSOM();
            }
        };


        // On Trained Model
        $scope.$on("onTrainedModel",
            function (event, args) {
                $scope.Data = args.message;
                $scope.ShowLoader = false;
                visualizeSOM();
                console.log("Data in second controller");
            });

        $scope.$on("onShowLoader",
            function (event, args) {
                d3.select("svg").remove();
                $scope.ShowLoader = args.message;
            });


    });