angular.module('app')
.controller('playersEditController', [
    '$scope', '$window', '$timeout', '$routeParams',
    'modals', 'playersFactory',
function (
    $scope, $window, $timeout, $routeParams,
    modals, playersFactory
) {
    // Grab our ID from the routed url
    $scope.PlayerID = $routeParams.id;

    // Data Setup
    $scope.formTitle = "Edit Player";
    $scope.emptyPlayer = {
        PlayerID: -1,
        FirstName: "",
        LastName: "",
        SkillLevel: "",
        Age: "",
        Email: "",
    };
    $scope.player = $scope.emptyPlayer;

    // Command Functions
    $scope.cancel = function () {
        $window.location.href = "index.html#!/home";
    };

    $scope.save = function () {
        if ($scope.formPlayer.$invalid) return;

        if ($scope.player.Age == "") {
            $scope.player.Age = -1;
        }
        playersFactory.save($scope.player)
        .then(function () {
            $window.location.href = "index.html#!/home";
        })
    };

    //Common Utility Functions
    function startWait() {
        $scope.bodyCursor = "wait-cursor";
    }

    function endWait() {
        $scope.bodyCursor = "";
    }

    // Set up and start our timer
    $scope.currentDate = new Date();
    $scope.tickInterval = 1000 //ms

    var tick = function () {
        $scope.currentDate = new Date(); // get the current time
        $timeout(tick, $scope.tickInterval); // reset the timer
    };

    $timeout(tick, $scope.tickInterval);

    // Load our data
    if ($scope.PlayerID > 0) {
        $scope.formTitle = "Edit Player";
        startWait();
        playersFactory.read($scope.PlayerID)
        .then(function (result) {
            $scope.player = result.data;
            if ($scope.player.Age < 1) {
                $scope.player.Age = "";
            }
            endWait();
        });
    } else {
        $scope.formTitle = "Add Player";
        $scope.player = $scope.emptyPlayer;
    }
}]);;
