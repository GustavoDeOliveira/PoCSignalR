import React, { Component } from "react";

export default class CanvasPointer extends Component {
    render() {
        const { target } = this.props;
        const divStyle = {
            position: 'relative',
            display: target? 'block':'none',
            width: '3px',
            height: '3px',
            borderRadius: '50%',
            backgroundColor: 'red',
            left: target?target.longitude*100 + '%': 0,
            top: target?target.lattitude*100 + '%': 0
        };
        return (<div style={divStyle}></div>);
    }
}