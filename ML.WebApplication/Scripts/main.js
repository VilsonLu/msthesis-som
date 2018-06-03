var app = angular.module("mlApp", []);

app.controller("configController",
    function ($scope, $http) {

        $scope.Configuration = {
            Epoch : 3,
            LearningRate: 0.5,
            Width: 0,
            Height: 0,
            FilePath: "",
            Labels: "",
            FeatureLabel: ""
        };

        $scope.Map = {};

        $scope.TrainModel = function () {

            var data = $scope.Configuration;
            var requestParams = {
                Epoch: data.Epoch,
                LearningRate: data.LearningRate,
                Width: data.Width,
                Height: data.Height,
                Labels: readCsvLine(data.Labels),
                FeatureLabel: data.FeatureLabel,
                FilePath: ""
            };

            var post = $http.post("http://localhost:49621/api/ML/GetTrainSOM", requestParams);

            post.then(function (response) {
                $scope.Map = response.data.Model;

                $scope.$broadcast("onTrainedModel", { message: $scope.Map });
                console.log("success call");
            });
        };

    });

app.controller("visualizationController",
    function ($scope) {

        $scope.Data = {};

        var nodes = [];
        var n = 20;
        var sen = 20;


        function getLabels() {
            return nodes.map(item => item.Label)
                .filter((value, index, self) => self.indexOf(value) === index);
        }

        var visualizeSOM = function () {

            d3.select("svg").remove();
            var svg = d3.select("#chart").append("svg").attr("width", 600).attr("height", 600),
                margin = 30,
                width = n * sen,
                height = n * sen
                ;

            var rgb_nodes = svg.append("g").attr("class", "nodes all");

            nodes = flatten($scope.Data.Map);
            labels = getLabels();

            assignColorToLabel(labels);

            rgb_nodes
                .selectAll("rect")
                .data(nodes)
                .enter().append("rect")
                .attr("x", function(node) { return node.Coordinate.X * sen; })
                .attr("y", function(node) { return node.Coordinate.Y * sen; })
                .attr("width", sen)
                .attr("height", sen)
                .attr("text", function(node) { return node.Label })
                .style("fill",
                function (node) {
                        return rgb(dictColor[node.Label]);
                    })
                .on("mouseover",
                    function(node) {
                        mover();
                        return tooltip.style("visibility", "visible")
                            .text(node.Label);
                    })
                .on("mousemove",
                    function() {
                        return tooltip.style("top", (event.pageY - 10) + "px").style("left", (event.pageX + 10) + "px");
                    })
                .on("mouseout",
                    function(node) {
                        mout();
                        return tooltip.style("visibility", "hidden");
                    });
        };


        // On Trained Model
        $scope.$on("onTrainedModel",
            function(event, args) {
                $scope.Data = args.message;
                visualizeSOM();
                console.log("Data in second controller");
            });

       

    });