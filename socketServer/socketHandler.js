const WebSocket = require("ws");
function Start(port){

    const wss = new WebSocket.Server({ port: port+1 },()=>{});

    var state = '{"newID":"" , "objects":{}}';
    
    wss.on('connection', socket=>{
        console.log('New client connected')
        socket.on('message', (data) => {
            var str = data.toString();
            console.log('Websocket data received \n %o',str);
            var message_type = str.substr(0,str.indexOf(' '));
            switch(message_type){
                case "new-client": socket.send(state); break;
                case "clear": {
                    state = '{"newID":"" , "objects":{}}';
                    wss.clients.forEach((socket) => {
                        socket.send("clear ");
                    });
                    break;
                }
                case "remove": {
                    var id = str.substr(str.indexOf(' ') + 1);
                    state = remove(id, state);
                    wss.clients.forEach((socket) => {
                        socket.send("remove " + id);
                    });
                    break;
                }
                default: {
                    state = str;
                    wss.clients.forEach((socket) => {
                        socket.send(state);
                    });
                }
            }
        })

        socket.on('close', () => {
            console.log("client has disconnected")
        });
    })

    wss.on('listening',()=>{
        console.log('listening on websocket on: '+(port+1));
    })
}

function remove(id, state) {
    const stateJson = JSON.parse(state);
    delete stateJson.objects[id];
    stateJson.newID = "";
    return JSON.stringify(stateJson);
}

module.exports = {
    Start: Start
};

