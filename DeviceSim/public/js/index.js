// attach ready event
function lightAON(){
            $("#lightA").show();
        }
function lightAOFF(){
            $("#lightA").hide();
        }
function lightBON(){
            $("#lightB").show();
        }
function lightBOFF(){
            $("#lightB").hide();
        }
function curtainOPEN(){
            $("#curtain").hide();
        }

function curtainCLOSE(){
            $("#curtain").show();
        }
function TVON(){
            $("#TV").get(0).play();
}

function TVOFF(){
            $("#TV").get(0).pause();
}

var lastaction ="";
var lastobject ="";
var lasturl="";
var lastverbose  = "";
function askForRenew(){
    $.ajax({
        url:'/renewpage',
        type:'POST',
        dataType:'json',
        data:{},
        success:function(result){
            var action = result.object;
            var object = result.action;
            var url = result.url;
            var verbose = result.verbose;
            if (lastaction == action && lastobject == object && lasturl == url && lastverbose == verbose)
                return;
            lastaction = action;
            lastobject = object;
            lasturl = url;
            lastverbose = verbose;
            console.log(result);
			
			document.getElementById("mediaDiv").style.display = "none";
			MediaPlayer.URL = "";
			MediaPlayer.controls.stop();
			document.getElementById("audioplayer").src = "";
			document.getElementById("hintmessage").style.top="500px";
			
            if (object == "MUSIC" || object == "RADIO"){
                document.getElementById("airmessage").style.display="none";
				console.log(typeof(action));
                if (action == 'START'){
                    //clear other audio
                    document.getElementById("radioframe").setAttribute("src","");

                    document.getElementById("audioplayer").setAttribute("src",url);
                    document.getElementById("audioplayer").play();
                    document.getElementById("hintmessage").style.display="";

                    if (object == "RADIO"){
						 document.getElementById("nowplaying").innerHTML = "" + verbose;
						 document.getElementById("mediaDiv").style.display = "";
						 document.getElementById("hintmessage").style.top="300px";
						 //document.getElementById("MediaPlayer").URL = url;
						 MediaPlayer.URL = url;
						 MediaPlayer.controls.play();
					}
                       
                    else{
                        document.getElementById("nowplaying").innerHTML = verbose;
					}
                    document.getElementById("nowplaying").setAttribute("href",url);

                }else if (action == "STOP"){
                    document.getElementById("audioplayer").src = "";
                    document.getElementById("hintmessage").style.display="none";
					document.getElementById("mediaDiv").style.display = "none";
					MediaPlayer.URL = "";
					MediaPlayer.controls.play();
                }
				
            }else if (object == "STORY"){
                if (action == "START"){
                    document.getElementById("audioplayer").setAttribute("src","");

                    document.getElementById("radioframe").setAttribute("src",url);
                    document.getElementById("hintmessage").style.display="";
                    document.getElementById("nowplaying").innerHTML = verbose;
                    document.getElementById("nowplaying").setAttribute("href",url);

                }else if (action == "STOP"){
                    document.getElementById("radioframe").setAttribute("src","");
                    document.getElementById("hintmessage").style.display="none";
                }
            }
            else if (object == "LIGHT"){
                if (action == "START"){
                    if (url == "客厅")
                        lightAON();
                    else if (url == "餐厅")
                        lightBON();
                    else {
                        lightAON();
                        lightBON();
                    }
                }
                else {
                    if (url == '客厅')
                        lightAOFF();
                    else if (url == "餐厅")
                        lightBOFF();
                    else {
                        lightAOFF();
                        lightBOFF();
                    }
                }
            }else if (object == "CURTAIN"){
                if (action == "START")
                    curtainOPEN();
                else
                    curtainCLOSE();
            }else if (object == "AIR"){
                document.getElementById("hintmessage").style.display="none";
                document.getElementById("airmessage").style.display="";
                if (action == "START"){
                    document.getElementById("airinfo").innerHTML = "空调已打开";
                }else if (action == "STOP"){
                    document.getElementById("airinfo").innerHTML = "空调已关闭";
                }if (action == "UP"){
                    document.getElementById("airinfo").innerHTML = "空调温度已经调高" + verbose;
                }if (action == "DOWN"){
                    document.getElementById("airinfo").innerHTML = "空调温度已经调低" + verbose;
                }if (action == "CHANGE"){
                    document.getElementById("airinfo").innerHTML = "空调温度已经设置为" + verbose;
                }
            }else if (object == "X"){
                document.getElementById("hintmessage").style.display ="none";
                document.getElementById("audioplayer").setAttribute("src","");
                document.getElementById("radioframe").setAttribute("src","");

            }
        }
    });
}


$(function(){
  $("#lightA").hide();
  $("#lightB").hide();
  $("#curtain").hide();
    setInterval("askForRenew()",500);
});


