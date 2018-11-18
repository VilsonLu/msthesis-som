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
            $scope.Region.TopLeft.X = 0;
            $scope.Region.TopRight.Y = 0;
            $scope.Region.BottomLeft.X = 0;
            $scope.Region.BottomLeft.Y = 0;
            $scope.Region.BottomRight.X = 0;
            $scope.Region.BottomRight.Y = 0;
        }
    });