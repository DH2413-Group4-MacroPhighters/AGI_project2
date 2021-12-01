const WebSocket = require("ws");
function Start(port){

    const wss = new WebSocket.Server({ port: port+1 },()=>{});

    var state = '{"newID":"" , "objects":{}}';
    
    wss.on('connection', socket=>{
        console.log('New client connected')
        socket.on('message', (data) => {
            console.log('Websocket data received \n %o',data.toString());
            switch(data.toString()){
                case "new-client": socket.send(state); break;
                case "clear": {
                    state = '{"newID":"" , "objects":{}}';
                    wss.clients.forEach((socket) => {
                        socket.send("clear");
                    });
                    break;
                } 
                default: {
                    wss.clients.forEach((socket) => {
                        state = data.toString();
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

module.exports = {
    Start: Start
};

