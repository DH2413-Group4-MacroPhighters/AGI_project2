const formidable = require ( 'formidable' );
const path = require("path");

function Start(app) {
    app.post('/mapPost', function (req, res) {
        console.log("The game has sent a new map: " + new Date());

        let form = new formidable.IncomingForm();
        form.uploadDir = path.join(__dirname, '..', 'frontEnd', 'uploads'); // This is the directory you have to create manually.

        form.on('fileBegin', function (name, file) {
            //rename the incoming file to the file's name
            file.filepath = form.uploadDir + "/" + file.originalFilename;
        });

        form.parse(req, function (err, fields, files) {
            if (err) {
                console.log('Some error: ', err);
            }
            console.log('Map Saved ');
        });
        res.send("Map upload completed")
    });
}

module.exports = {
    Start: Start
};

