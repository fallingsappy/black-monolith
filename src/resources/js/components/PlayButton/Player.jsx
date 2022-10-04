import React, { Component } from "react";
import "./Player.css";
import ambient from "../../../music/MK4.mp3";
import ReactAudioPlayer from "react-audio-player";

class Player extends Component {
  render() {
    return (
      <div className="player">
        <ReactAudioPlayer
          controls
          controlsList="nodownload nofullscreen noremoteplayback"
          src={ambient}
          loop
          volume={0.3}
        />
      </div>
    );
  }
}

export default Player;
