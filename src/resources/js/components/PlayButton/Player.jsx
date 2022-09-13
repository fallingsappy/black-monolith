import React, { Component } from "react";
import Play from "./Play";
import Pause from "./Pause";
import "./Player.css";
import SoundBar from "../Sound/SoundBar";

class Player extends Component {
  constructor(props) {
    super(props);
    this.state = {
      playing: false,
    };
  }

  handlePlayerClick = () => {
    if (!this.state.playing) {
      this.setState({ playing: true });
    } else {
      this.setState({ playing: false });
    }
  };

  render() {
    return (
      <div className="player">
        <SoundBar isPlaying={this.state.playing} />
        {this.state.playing ? (
          <Pause onPlayerClick={this.handlePlayerClick} />
        ) : (
          <Play onPlayerClick={this.handlePlayerClick} />
        )}
      </div>
    );
  }
}

export default Player;
