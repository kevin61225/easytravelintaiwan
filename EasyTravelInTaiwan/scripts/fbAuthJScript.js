function FacebookInit() {
    FB.init({
        appId: '544818928892863', // App ID
        channelUrl: 'http://localhost:51145/', // Channel File
        status: true, // check login status
        cookie: true, // enable cookies to allow the server to access the session
        xfbml: true  // parse XFBML
    });
}

function FacebookLogin() {
    FacebookInit();
    FB.login(function (response) {
        if (response.authResponse) {
            FB.api('/me', function (response) {
                var fbID = response.id;
                var fbName = response.name;
                var fbEmail = response.email;
                location.href = '/Member/FacebookLogin?fbID=' + fbID + '&fbName=' + fbName + '&fbEmail=' + fbEmail;
                console.log('login ! Good to see you, ' + fbName + ', ' + fbID + ', ' + 'Email = ' + fbEmail);
            });
            
        }
    }, { scope: 'email' });
}

// Load the SDK asynchronously
(function (d) {
    var js, id = 'facebook-jssdk', ref = d.getElementsByTagName('script')[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement('script'); js.id = id; js.async = true;
    js.src = "//connect.facebook.net/en_US/all.js";
    ref.parentNode.insertBefore(js, ref);
}(document));


function Logout() {
    FacebookLogout();
    location.href = '/Home/LogOut';
}

function FacebookLogout() {
    FacebookInit();
    FB.getLoginStatus(statusResponse);
    console.log("Logged out !");
}

function statusResponse(response) {
    FacebookInit();
    FB.logout(response.authResponse.accessToken);
}




