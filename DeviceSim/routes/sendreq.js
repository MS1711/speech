
var express = require('express');
var router = express.Router();
var fs = require('fs');
router.get('/',function (req, res){

	var url = require('url');
	var url_parts = url.parse(req.url, true);
	var query = url_parts.query;
	var data = query["txt"];
	
    fs.writeFile('C:/workspace/newTemp.txt', data, "utf8", function(err){
        if(err) throw err;
		res.json({'status':true});
    });
	
	res
});

module.exports = router;