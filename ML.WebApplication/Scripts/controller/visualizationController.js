app.controller("visualizationController",
    function ($scope) {

        $scope.Data = {};
        $scope.Trajectory = [];
        $scope.ShowLoader = false;
        $scope.ShowError = false;
        var nodes = [];
        var n = 20;
        var sen = 50;

        var svg = {};


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
            svg = d3.select("#chart").append("svg").attr("width", h).attr("height", w),
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
                .attr("x", function (node) { return node.Coordinate.X * sen; })
                .attr("y", function (node) { return node.Coordinate.Y * sen; })
                .attr("width", sen)
                .attr("height", sen)
                .style("fill",
                function (node) {
                    return rgb(dictColor[node.Label]);
                });
            //.on("mouseover",
            //function (node) {
            //    mover();
            //    return tooltip.style("visibility", "visible")
            //        .text(node.Label);
            //})
            //.on("mousemove",
            //function () {
            //    return tooltip.style("top", (event.pageY - 10) + "px").style("left", (event.pageX + 10) + "px");
            //})
            //.on("mouseout",
            //function (node) {
            //    mout();
            //    return tooltip.style("visibility", "hidden");
            //});

            var text = rgb_nodes.selectAll("text")
                .data(nodes)
                .enter()
                .append("text");

            var textLabels = text
                .attr("x", function (node) { return coordinateMapper(node.Coordinate.X); })
                .attr("y", function (node) { return coordinateMapper(node.Coordinate.Y); })
                .text(function (node) { return  node.Label; })
                .attr("font-family", "sans-serif")
                .attr("font-size", "15px")
                .attr("fill", "black");

        };

        var coordinateMapper = function (d) {
            return d * sen + 0.5 * sen
        }

        var plotTrajectory = function () {

            var data = $scope.Trajectory.Trajectories;
            var newdata = data.map((p, index) => index === data.length - 1 ? [p] : [p, data[index + 1]]);
            var dataLength = data.length;

            var index = 0;



            var interpolator = function (d) {
                return d / dataLength;
            }

            var timeInterpolator = function (d) {
                var interpolatedNum = interpolator(d);
                var color = d3.interpolateLab("yellow", "red")(interpolatedNum);
                console.log(`${d} : ${interpolatedNum} : ${color} `);
                return color;
            }


            

            var lineFunction = d3.line()
                .x(function (d) { return coordinateMapper(d.Node.Coordinate.X); })
                .y(function (d) { return coordinateMapper(d.Node.Coordinate.Y); })
                .curve(d3.curveLinear);

            //The line SVG Path we draw
            var lineGraph = svg.selectAll("path")
                .data(newdata).enter().append("path")
                .attr("d", p => lineFunction(p))
                .attr("stroke", p => timeInterpolator(p[0].Instance.OrderNo))
                .attr("stroke-width", 5)
                .attr("fill", "none");

            svg.selectAll("circle")
                .data(data).enter()
                .append("circle")
                .attr("cx", function (d) { return coordinateMapper(d.Node.Coordinate.X); })
                .attr("cy", function (d) { return coordinateMapper(d.Node.Coordinate.Y); })
                .attr("r", "6px")
                .attr("fill", p => timeInterpolator(p.Instance.OrderNo));

        }

        var visualizeCluster = function () {
            d3.select("svg").remove();
            var w = $scope.Data.Width * sen;
            var h = $scope.Data.Height * sen;
            svg = d3.select("#chart").append("svg").attr("width", w).attr("height", h),
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
                });


            var text = rgb_nodes.selectAll("text")
                .data(nodes)
                .enter()
                .append("text");

            var textLabels = text
                .attr("x", function (node) { return coordinateMapper(node.Coordinate.X); })
                .attr("y", function (node) { return coordinateMapper(node.Coordinate.Y); })
                .text(function (node) { return node.ClusterLabel; })
                .attr("font-family", "sans-serif")
                .attr("font-size", "15px")
                .attr("fill", "black");
            //.on("mouseover",
            //function (node) {
            //    mover();
            //    return tooltip.style("visibility", "visible")
            //        .text(node.Label);
            //})
            //.on("mousemove",
            //function () {
            //    return tooltip.style("top", (event.pageY - 10) + "px")
            //        .style("left", (event.pageX + 10) + "px");
            //})
            //.on("mouseout",
            //function (node) {
            //    mout();
            //    return tooltip.style("visibility", "hidden");
            //});
        }


        // Shows the cluster group
        $scope.isShowCluster = false;
        $scope.showCluster = function () {
            if ($scope.isShowCluster) {
                visualizeCluster();
            } else {
                visualizeSOM();
            }

            if ($scope.Trajectory === undefined) {
                return;
            }

            if ($scope.Trajectory.Trajectories.length > 0) {
                plotTrajectory();
            }
        };


        // Events
        $scope.$on("onTrainedModel",
            function (event, args) {
                $scope.Data = args.message;
                $scope.ShowLoader = false;
                visualizeSOM();
                console.log("Visualizing SOM");
            });

        $scope.$on("onPlotTrajectory",
            function (event, args) {
                $scope.Trajectory = args.trajectory;
                plotTrajectory();
                console.log("Plotting Trajectory");
            });

        $scope.$on("onShowLoader",
            function (event, args) {
                d3.select("svg").remove();
                $scope.ShowLoader = args.message;
            });

        $scope.$on("onShowError", function (event, args) {
            $scope.ShowError = args.message;
        });


    });