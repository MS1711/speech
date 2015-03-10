
var express = require('express');
var router = express.Router();
var fs = require('fs');
router.post('/',function (req, res){

    var commands = [];
    var linenum = 0;
    fs.readFile('C:/workspace/newTemp.txt','utf-8',function(err,data){
        if(err){
            console.error(err);
        }
        else{
            commands = data.split('\n');
            for (var i = 0; i < commands.length; i++){
                if (commands[i][0] != 'X'){
                    linenum = i;
                    action = commands[i].split('$');
                    res.json({'object':action[0],'action':action[1],'url':action[2],'verbose':action[3]});
                    //console.log(res);
                    break;
                }
            }
        }
    });
    /*
    var towrite = "";
    for (var i = 0; i < commands.length; i++){
        if (i != linenum){
            towrite += commands[i];
            towrite += '\n';
        }else{
            towrite += "X";
            towrite += commands[i];
            towrite += '\n';
        }
    }

    fs.writeFile('E:/newTemp.txt', towrite, function(err){
        if(err) throw err;
        console.log('内容被覆盖');
    });
*/


});

module.exports = router;