import React, {useEffect, useRef} from "react";
import * as THREE from "three";
import {toRadians} from "../../../Helpers/Geometry";
import ambient from "../../../music/dark-ambient.mp3"
import Sound from 'react-sound';
import geiger from "../../../music/geiger.wav";
import "../../../css/firefly.sass";
import "../../../css/fog.css";

var fireflies = [];
var quantity = 50;
renderFireFlies();

function Monolith () {
    const innerRef = useRef(null);

    useEffect(() => {
        const scene = new THREE.Scene();
        {
            const near = 4;
            const far = 5;
            const color = "rgba(128,128,128,1)";
            scene.fog = new THREE.Fog(color, near, far);
            scene.background = new THREE.Color("rgba(128,128,128,1)");
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
        scene.add( light );

        const geometry = new THREE.BoxGeometry(2, 4, 0.4);
        const loader = new THREE.TextureLoader();
        const material = new THREE.MeshPhongMaterial({
            map: loader.load('https://www.google.com/url?sa=i&url=https%3A%2F%2Fwww.stoneinternational.it%2Fen%2Fnatural-stones%2Fblack-granite.32&psig=AOvVaw1V-clZ-qcIaFhrsp6jt_Vr&ust=1595843859299000&source=images&cd=vfe&ved=0CAIQjRxqFwoTCIjWquXT6uoCFQAAAAAdAAAAABAD'),
        });
        const cube = new THREE.Mesh(geometry, material);
        scene.add(cube);

        var orbit = new THREE.Object3D();
        orbit.rotation.order = "YXZ"; //this is important to keep level, so Z should be the last axis to rotate in order...
        orbit.position.copy( cube.position );
        orbit.rotation.x = -10;
        scene.add(orbit );


        let isDragging = false;
        let previousMousePosition = {
            x: 0,
        };

        (renderer.domElement).addEventListener('mousedown', function(e){
            isDragging = true;
        });
        (renderer.domElement).addEventListener('mousemove', function(e) {
            const deltaMove = {
                x: e.offsetX - previousMousePosition.x,
            };

            if(isDragging) {

                let scale = -0.01;
                orbit.rotateY( e.movementX * scale );
                orbit.rotation.x = -10;
                orbit.rotation.z = 0; //this is important to keep the camera level..

                const deltaRotationQuaternion = new THREE.Quaternion()
                    .setFromEuler(new THREE.Euler(
                        toRadians(deltaMove.y * 1),
                        toRadians(deltaMove.x),
                        0,
                        'XYZ'
                    ));

                cube.quaternion.multiplyQuaternions(deltaRotationQuaternion, cube.quaternion);
            }

            previousMousePosition = {
                x: e.offsetX,
            };
        });

        div.addEventListener('mouseup', function(e){

            isDragging = false;
        });

        window.requestAnimFrame = (function(){
            return  window.requestAnimationFrame ||
                window.webkitRequestAnimationFrame ||
                window.mozRequestAnimationFrame ||
                function(callback) {
                    window.setTimeout(callback, 1000 / 60);
                };
        })();

        camera.position.z = 5;

        function render() {

            renderer.render( scene, camera );

            cube.rotation.y += 0.001;

            window.requestAnimFrame( render );
        }

        orbit.add( camera );
        render();
    },[])

    return (
        <>
        <Sound
            url={ambient}
            playStatus={Sound.status.PLAYING}
            autoLoad
            loop
            volume={10}
        />
        <Sound
            url={geiger}
            playStatus={Sound.status.PLAYING}
            autoLoad
            loop
            volume={2}
        />
            <div className="rotator" ref={innerRef}>
                <div id="foglayer_01" className="fog">
                    <div className="image01"/>
                    <div className="image02"/>
                </div>
                <div id="foglayer_02" className="fog">
                    <div className="image01"/>
                    <div className="image02"/>
                </div>
                <div id="foglayer_03" className="fog">
                    <div className="image01"/>
                    <div className="image02"/>
                </div>
            </div>
            {fireflies}

        </>
    )
}

function renderFireFlies() {
    for (var i = 1; i <= quantity; i++) {
        fireflies.push(<div className="firefly"/>)
    }
}
export default Monolith;
