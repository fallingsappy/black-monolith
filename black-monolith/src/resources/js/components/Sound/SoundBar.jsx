import React, { useEffect, useRef } from "react";
import ambient from "../../../music/MK4.mp3";
import "../../../css/firefly.sass";
import ReactAudioPlayer from "react-audio-player";

function SoundBar(props) {
  const player = useRef();

  useEffect(() => {
    if (props.isPlaying === true) {
      player.current.audioEl.current.play();
    } else {
      player.current.audioEl.current.pause();
    }
  }, [props.isPlaying]);

  return (
    <>
      <ReactAudioPlayer src={ambient} autoPlay loop volume={0.3} ref={player} />
    </>
  );
}

export default SoundBar;
