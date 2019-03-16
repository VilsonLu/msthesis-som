app.controller("configController",
    function ($scope, $http) {

        // Properties
        $scope.Configuration = {
            Epoch: 3,
            LearningRate: 0.5,
            Width: 0,
            Height: 0,
            FilePath: "",
            Labels: "",
            FeatureLabel: "",
            KMeans: 2,
            Regions: []
        };

        $scope.Files = [];

        $scope.Map = {};
        $scope.Trajectories = [];

        $scope.Region = {
            Label: "",
            TopLeft: {
                X: 0,
                Y: 0
            },
            TopRight: {
                X: 0,
                Y: 0
            },
            BottomLeft: {
                X: 0,
                Y: 0
            },
            BottomRight: {
                X: 0,
                Y: 0
            }
        }

        // Methods
        $scope.TrainModel = function () {

            var data = $scope.Configuration;

            $scope.$broadcast("onShowLoader", { message: true });

            $http({
                method: "POST",
                url: "http://localhost:49621/api/ML/GetTrainSOM",
                headers: { "Content-Type": undefined },

                transformRequest: function (data) {
                    var formData = new FormData();
                    formData.append("model", angular.toJson(data.model));
                    for (var i = 0; i < data.files.length; i++) {
                        formData.append("file" + i, data.files[i]);
                    }
                    return formData;
                },
                data: { model: $scope.Configuration, files: $scope.Files }

            }).then(
                function (response) {
                    $scope.Map = response.data.Model;
                    $scope.$broadcast("onTrainedModel", { message: $scope.Map });
                    $scope.$broadcast("onShowError", { message: false });
                },
                function (error) {
                    $scope.$broadcast("onShowLoader", { message: false });
                    $scope.$broadcast("onShowError", { message: true });
                });
        };

        $scope.AddRegion = function () {
            var r = JSON.parse(JSON.stringify($scope.Region));
            $scope.Configuration.Regions.push(r);
            resetRegion();
        }

        $scope.RemoveRegions = function () {
            $scope.Configuration.Regions = [];
            resetRegion();
        }

        $scope.ExportModel = function () {
            $scope.Map.Dataset = null;
            var blob = new Blob([JSON.stringify($scope.Map, undefined, 2)], { type: 'text/plain;charset=utf-8' });
            window.saveAs(blob, `Map_${$scope.Map.MapId}.json`);
        }

        $scope.ImportModel = function () {
            var impportFile = $scope.Files[0];

            var fileReader = new FileReader();

            fileReader.addEventListener('loadend', (e) => {
                $scope.Map = JSON.parse(e.srcElement.result);
                $scope.Configuration.LearningRate = $scope.Map.ConstantLearningRate;
                $scope.Configuration.Width = $scope.Map.Width;
                $scope.Configuration.Height = $scope.Map.Height;
                $scope.Configuration.Epoch = $scope.Map.Epoch;
                $scope.$broadcast("onTrainedModel", { message: $scope.Map });
            });

            fileReader.readAsText(impportFile);
        }

        $scope.PlotTrajectory = () => {

            var data = new FormData();
            data.append("map", angular.toJson($scope.Map));
            data.append("file", $scope.Files[1]);

            $http({
                method: "POST",
                url: "http://localhost:49621/api/ML/PlotTrajectory",
                data: data,
                headers: { 'Content-Type': undefined }, //this is important
                transformRequest: angular.identity  //also important
            }).then(
                function (response) {


                    var length = response.data;

                    $scope.Trajectories = response.data;
                    $scope.$broadcast("onPlotTrajectory", { trajectory: $scope.Trajectories });
                },
                function (error) {
                    console.log('load failed');
                });

            $scope.Files = [];
        };

        // Events

        $scope.$on("selectedFile", function (event, args) {
            $scope.$apply(function () {
                //add the file object to the scope's files collection  
                $scope.Files.push(args.file);
            });
        });

        var resetRegion = function () {
            $scope.Region.Label = "";
            $scope.Region.TopLeft.X = 0;
            $scope.Region.TopLeft.Y = 0;
            $scope.Region.TopRight.X = 0;
            $scope.Region.TopRight.Y = 0;
            $scope.Region.BottomLeft.X = 0;
            $scope.Region.BottomLeft.Y = 0;
            $scope.Region.BottomRight.X = 0;
            $scope.Region.BottomRight.Y = 0;
        }
    });