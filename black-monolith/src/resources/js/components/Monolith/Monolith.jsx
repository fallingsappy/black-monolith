import React, { useCallback, useEffect, useRef } from "react";
import * as THREE from "three";
import THREEx from "../../libs/threex.domevents";
import { throttle } from "lodash-es";
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
      scene.fog = new THREE.Fog(mainColor, near, far);
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
    insideUniforms.glowColor.value.set("lightgreen");
    const outsideUniforms = glowMesh.outsideMesh.material.uniforms;
    outsideUniforms.glowColor.value.set("lightgreen");

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
    domEvents.addEventListener(cube, "click", openModal, false);
    domEvents.addEventListener(cube, "touchstart", openModal, false);

    domEvents.addEventListener(
      cube,
      "mouseover",
      function () {
        cube.add(glowMesh.object3d);
      },
      false
    );
    domEvents.addEventListener(
      cube,
      "mouseout",
      function () {
        cube.remove(glowMesh.object3d);
      },
      false
    );

    const resizeUpdateInterval = 500;

    window.addEventListener(
      "resize",
      throttle(
        () => {
          const width = window.innerWidth;
          const height = window.innerHeight;
          camera.aspect = width / height;
          camera.updateProjectionMatrix();
          renderer.setSize(width, height);
          setCanvasDimensions(renderer.domElement, width, height);
        },
        resizeUpdateInterval,
        { trailing: true }
      )
    );

    renderer.domElement.addEventListener("mousedown", function () {
      isDragging = true;
    });
    renderer.domElement.addEventListener("touchstart", function () {
      isDragging = true;
    });

    renderer.domElement.addEventListener("mousemove", handleDrag);
    renderer.domElement.addEventListener("pointermove", handleDrag);

    div.addEventListener("mouseup", function () {
      isDragging = false;
    });
    div.addEventListener("touchend", function () {
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

    function openModal() {
      props.toggleModal(true);
    }

    function render() {
      renderer.render(scene, camera);

      cube.rotation.y += 0.001;

      window.requestAnimFrame(render);
    }

    function handleDrag(e) {
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
    }

    function setCanvasDimensions(
      canvas,
      width,
      height,
      set2dTransform = false
    ) {
      const ratio = window.devicePixelRatio;
      canvas.width = width * ratio;
      canvas.height = height * ratio;
      canvas.style.width = `${width}px`;
      canvas.style.height = `${height}px`;
      if (set2dTransform) {
        canvas.getContext("2d").setTransform(ratio, 0, 0, ratio, 0, 0);
      }
    }

    orbit.add(camera);

    render();
  }, [props]);

  return (
    <>
      <Player />
      <div ref={innerRef} />
      {renderFireFlies()}
    </>
  );
}

export default Monolith;
