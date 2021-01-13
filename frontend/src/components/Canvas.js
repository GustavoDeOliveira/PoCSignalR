import React, { Component } from "react";
import CanvasPointer from "./CanvasPointer";

export default class Canvas extends Component {
    render() {
        return (
        <div>
            <h2>{this.props.title}</h2>
            <div style={{
                        height: '240px',
                        padding: 0,
                        border: '1px solid black',
                        boxSizing: 'border-box',
                        backgroundColor: 'white'
                }} onMouseMove={this.props.handleMouseMove}>
                    <CanvasPointer target={this.props.target} />
                </div>
        </div>
        );
    }
}