import React, { Component } from "react";
import { HubConnectionBuilder } from '@microsoft/signalr';
import Canvas from "./components/Canvas";
import "./app.css";

export default class App extends Component {
    state = {
        connection: null,
        user: '',
        ucoordinates: {userId: '', lattitude: 0.0, longitude: 0.0},
        target: '',
        tcoordinates: {userId: '', lattitude: 0.0, longitude: 0.0},
        token: ''
        // updateInterval: null
    };
    baseurl = "https://apipocsignalr.azurewebsites.net/";
    // baseurl = "http://localhost:5000/";
    apiurl = this.baseurl + "api";
    huburl = this.baseurl + "hub/coordinates";
    // updateTimems = 500;

    connectionBuilder = () => {
        const options = {
            accessTokenFactory: () => {
              return this.state.token;
            }
            // httpClient: {
            //     post: (url, httpOptions) => {
            //       const headers = {
            //         ...httpOptions.headers,
            //         Authorization: this.state.token
            //       };

            //       const init = {
            //           method: "POST",
            //           headers
            //       };
            
            //       return fetch(url, init).then(response => {
            //         return response;
            //       });
            //     }
            //   }
        };
        return new HubConnectionBuilder()
          .withUrl(this.huburl, options)
          .withAutomaticReconnect();
    };

    connect = () => {
        const connection = this.connectionBuilder().build();
        connection.on("ReceiveMessage", message => console.log("Coordinates.ReceiveMessage: " + message));
        connection.on("ReceiveCoordinates", message => {
            console.log("Coordinates.ReceiveCoordinates: " + message);
            const tcoordinates = JSON.parse(message);
            this.setState({tcoordinates});
        });
        connection.start().then(() => {
            console.log("Hub Coordinates Conectado!");
            this.setState({connection});
        }).catch(e => console.error(e));
        this.setState({connection});
    };

    disconnect = () => {
        this.state.connection.stop();
    };

    handleTrack = e => {
        this.handleSubmit(e, response => {
            if (response.ok) {
                const tcoordinates = this.state.tcoordinates;
                tcoordinates.userId = this.state.target;
                this.setState({...this.state, tcoordinates});
                response.json().then(json => {
                    console.log(json.message);
                });
            }
        });
    };

    handleLogin = e => {
        this.handleSubmit(e, (response => {
            if (response.ok) {
                response.json().then(json => {
                    const ucoordinates = this.state.ucoordinates;
                    ucoordinates.userId = this.state.user;
                    this.setState({...this.state, ucoordinates});
                    this.setState({token: json.token});
                    this.connect();
                    console.log(json.message);
                });
            }
        }));
    };

    handleSignIn = e => {
        this.handleSubmit(e, (response => {
            if (response.ok) {
                response.json().then(json => alert(json.message));
            } else {
                response.json().then(json => {
                    if (json.errors) {
                        if (json.errors.forEach) {
                            json.message += "\nErro(s):";
                            json.errors.forEach(err => {
                                json.message += "\n" + JSON.stringify(err);
                            });
                            alert(json.message);
                        } else {
                            json.title += "\nErro(s):";
                            Object.values(json.errors).forEach(err => {
                                json.title += "\n" + JSON.stringify(err);
                            });
                            alert(json.title);
                        }
                    }
                });
            }
        }), true);
    };

    handleLogout = e => {
        this.handleSubmit(e, (response) => {
            if (response.ok) {
                this.setState({
                    token: '',
                    user: '',
                    ucoordinates: {userId: '', lattitude: 0.0, longitude: 0.0},
                    target: '',
                    tcoordinates: {userId: '', lattitude: 0.0, longitude: 0.0}
                });
                this.disconnect();
                response.json().then(json => {
                    console.log(json.message);
                });
            }
        });
    };

    handleSubmit = (e, action, keepForm) => {
        e.preventDefault();
        const form = e.target;
        const headers = new Headers();
        headers.append("content-type", "text/json");
        if (this.state.token)
            headers.append("Authorization", "Bearer " + this.state.token);
        const init = {
            method: form.method,
            headers,
            body: JSON.stringify(Object.fromEntries(new FormData(form).entries()))
        };
        if (!keepForm)
            for (var i = 0; i < form.elements.length; i++) {
                const el = form.elements[i];
                if (el.type !== 'submit') el.value = "";
            }
        
        fetch(form.action, init).then(action).catch(reason => console.log(reason));
    }

    handleChange = e => {
        this.setState({...this.state, [e.target.title]: e.target.value});
    };

    handleMouseMove = e => {
        const docw = parseInt(e.target.offsetWidth);
        const doch = parseInt(e.target.offsetHeight);
        const ucoordinates = {...this.state.ucoordinates, 
            longitude: parseFloat(e.nativeEvent.offsetX)/docw,
            lattitude: parseFloat(e.nativeEvent.offsetY)/doch};
        this.setState({ucoordinates});
        this.updateCoordinates();
    };

    updateCoordinates = () => {
        const { ucoordinates, target, connection } = this.state;
        if (connection?connection.connectionId:false) {
          if (ucoordinates.userId) {
            ucoordinates.targetId = target;
            connection.invoke("Update", ucoordinates)
                .then(response => console.log("Coordinates.UpdateCoordinates: " + response))
                .catch(reason => console.log("Err Coordinates.UpdateCoordinates: " + reason));
          }
        }
    };

    // stopUpdates = () => {
    //     clearInterval(this.state.updateInterval);
    //     this.setState({updateInterval: null});
    // };

    // startUpdates = () => {
    //     if (this.state.updateInterval) clearInterval(this.state.updateInterval);
    //     this.setState({updateInterval: setInterval(this.updateCoordinates, this.updateTimems)});
    // };

    render() {
        const formStyle = {
          display: 'flex'
        };
        const inputStyle = {
          margin: '0 .1em',
          flex: 5
        };
        const signInStyle = {...inputStyle, 
            fontWeight: 'bold', 
            fontSize: '20px',
            padding: '.2em',
            border: 'outset 2px',
            borderRadius: '0 0 25% 25%'
        };
        return (
        <div className="app">
            <h1>Mouse Tracker</h1>
            {!this.state.token?
              <div>
                  <h3>Cadastro</h3>
              <form onSubmit={this.handleSignIn} 
                method="POST" 
                action={this.apiurl + "/account/signin"}>
                    <div style={formStyle}>
                        <label htmlFor="user">Usuário: </label>
                        <input required="required" style={{...inputStyle, flex: 7}} 
                            type="text" name="userName" title="user"
                            autoComplete="off"
                            placeholder="Digite seu nome de Usuário" />
                        <label htmlFor="target">Senha: </label>
                        <input required="required" style={{...inputStyle, flex: 3}} 
                            autoComplete="off"
                            type="password" name="password"
                            placeholder="Digite sua senha" />
                    </div>
                    <div style={formStyle}>
                        <label htmlFor="email">Email: </label>
                        <input required="required" style={{...inputStyle, flex: 7}} 
                            autoComplete="off"
                            type="text" name="email"
                            placeholder="Digite seu e-mail" />
                        <label htmlFor="confirmPassword">Confirmar Senha: </label>
                        <input required="required" style={{...inputStyle, flex: 3}} 
                            autoComplete="off"
                            type="password" name="confirmPassword"
                            placeholder="Confirme sua senha" />
                    </div>
                    <div style={formStyle}>
                        <input style={signInStyle} type="submit" value="Cadastrar-se" />
                    </div>
              </form>
              <h3>Entrar</h3>
              <form style={formStyle} 
                onSubmit={this.handleLogin} 
                method="POST" 
                action={this.apiurl + "/account/login"}>
                <label htmlFor="user">Usuário: </label>
                  <input required="required" style={{...inputStyle, flex: 7}} 
                  type="text" name="userName" title="user"
                  autoComplete="off"
                  value={this.state.user} 
                  placeholder="Digite seu nome de Usuário"
                  onChange={this.handleChange} />
                <label htmlFor="target">Senha: </label>
                  <input required="required" style={{...inputStyle, flex: 3}} 
                  type="password" name="password"
                  autoComplete="off"
                  placeholder="Digite sua senha" />
                  <input style={inputStyle} type="submit" value="Entrar" />
              </form>
              </div>
              : //Se houver um token, usuário está logado. Mostrar botão de logout
              <form style={formStyle} 
                onSubmit={this.handleLogout} 
                method="POST" 
                action={this.apiurl + "/account/logout"}>
                  <label style={{flex: 7}}>Bem vindo, {this.state.ucoordinates.userId}! </label>
                  <input style={{...inputStyle, flex: 3}} type="submit" value="Sair" />
              </form>
            }

            <Canvas title="Você" handleMouseMove={this.handleMouseMove} />

            <form style={formStyle} 
              onSubmit={this.handleTrack} 
              method="POST" 
              action={this.apiurl + "/track"}>
                <label htmlFor="target">Alvo: </label>
                <input required="required" style={{...inputStyle, flex: 7}} type="text" name="target" title="target"
                value={this.state.target} 
                placeholder="Nome de usuário do alvo"
                onChange={this.handleChange} />
                <input style={{...inputStyle, flex: 3}} type="submit" value="Enviar" />
            </form>

            <Canvas title={'Alvo: ' + this.state.tcoordinates.userId} target={this.state.tcoordinates} />
        </div>
        );
    }
}