import React, { useCallback, useEffect, useRef } from "react";
import * as THREE from "three";
import THREEx from "../../libs/threex.domevents";

import { toRadians } from "../../../Helpers/Geometry";
import "../../../css/firefly.sass";
import { GeometricGlowMesh } from "../../libs/threex.geometricglowmesh";
import Player from "../PlayButton/Player";

const quantity = 50;
const mainColor = "rgba(164,61,224,1)";

function Monolith(props) {
  const innerRef = useRef(null);

  const renderFireFlies = useCallback(() => {
    const fireflies = [];

    for (let i = 1; i <= quantity; i++) {
      fireflies.push(<div className="firefly" />);
    }

    return fireflies;
  }, []);

  useEffect(() => {
    const scene = new THREE.Scene();
    {
      const near = 4;
      const far = 5;
      const color = mainColor;
      scene.fog = new THREE.Fog(color, near, far);
      scene.background = new THREE.Color(mainColor);
    }

    const camera = new THREE.PerspectiveCamera(
      75,
      window.innerWidth / window.innerHeight,
      0.1,
      1000
    );

    const div = innerRef.current;

    const renderer = new THREE.WebGLRenderer();
    renderer.setSize(window.innerWidth, window.innerHeight);
    div.appendChild(renderer.domElement);

    const light = new THREE.AmbientLight(0x404040, 6); // soft white light
    scene.add(light);

    const geometry = new THREE.BoxGeometry(2, 4, 0.4);

    const material = new THREE.MeshBasicMaterial({
      color: new THREE.Color("black"),
    });
    const cube = new THREE.Mesh(geometry, material);

    scene.add(cube);

    // create a glowMesh
    const glowMesh = new GeometricGlowMesh(cube);

    //////////////////////////////////////////////////////////////////////////////////
    //		customize glow mesh if needed					//
    //////////////////////////////////////////////////////////////////////////////////

    // example of customization of the default glowMesh
    const insideUniforms = glowMesh.insideMesh.material.uniforms;
    insideUniforms.glowColor.value.set("hotpink");
    const outsideUniforms = glowMesh.outsideMesh.material.uniforms;
    outsideUniforms.glowColor.value.set("hotpink");

    const orbit = new THREE.Object3D();
    orbit.rotation.order = "YXZ"; //this is important to keep level, so Z should be the last axis to rotate in order...
    orbit.position.copy(cube.position);
    orbit.rotation.x = -10;
    scene.add(orbit);

    let isDragging = false;
    let previousMousePosition = {
      x: 0,
    };

    const domEvents = new THREEx.DomEvents(camera, renderer.domElement);
    // add an event listener for this callback
    domEvents.addEventListener(
        cube,
        "click",
        function (e) {
          props.toggleModal(true);
        },
        false
    );
    domEvents.addEventListener(
      cube,
      "mouseover",
      function (e) {
        cube.add(glowMesh.object3d);
      },
      false
    );
    domEvents.addEventListener(
      cube,
      "mouseout",
      function (e) {
        cube.remove(glowMesh.object3d);
      },
      false
    );

    renderer.domElement.addEventListener("mousedown", function (e) {
      isDragging = true;
    });
    renderer.domElement.addEventListener("mousemove", function (e) {
      const deltaMove = {
        x: e.offsetX - previousMousePosition.x,
      };

      if (isDragging) {
        let scale = -0.01;
        orbit.rotateY(e.movementX * scale);
        orbit.rotation.x = -10;
        orbit.rotation.z = 0; //this is important to keep the camera level..

        const deltaRotationQuaternion = new THREE.Quaternion().setFromEuler(
          new THREE.Euler(
            toRadians(deltaMove.y * 1),
            toRadians(deltaMove.x),
            0,
            "XYZ"
          )
        );

        cube.quaternion.multiplyQuaternions(
          deltaRotationQuaternion,
          cube.quaternion
        );
      }

      previousMousePosition = {
        x: e.offsetX,
      };
    });

    div.addEventListener("mouseup", function (e) {
      isDragging = false;
    });

    window.requestAnimFrame = (function () {
      return (
        window.requestAnimationFrame ||
        window.webkitRequestAnimationFrame ||
        window.mozRequestAnimationFrame ||
        function (callback) {
          window.setTimeout(callback, 1000 / 60);
        }
      );
    })();

    camera.position.z = 5;

    function render() {
      renderer.render(scene, camera);

      cube.rotation.y += 0.001;

      window.requestAnimFrame(render);
    }

    orbit.add(camera);

    render();
  }, []);

  return (
    <>
      <Player />
      <div ref={innerRef} />
      {renderFireFlies()}
    </>
  );
}

export default Monolith;
