import * as THREE from "three";
import { createAtmosphereMaterial } from "./threex.atmospherematerial";
import { dilateGeometry } from "./threex.dilategeometry";

export const GeometricGlowMesh = function (mesh, addInner = false) {
  var object3d = new THREE.Object3D();

  var geometry = mesh.geometry.clone();
  dilateGeometry(geometry, 0.01);
  var material = createAtmosphereMaterial();
  material.uniforms.glowColor.value = new THREE.Color("cyan");
  material.uniforms.coeficient.value = 1.1;
  material.uniforms.power.value = 1.4;
  var insideMesh = new THREE.Mesh(geometry, material);

  if (addInner) {
    object3d.add(insideMesh);
  }

  var geometry = mesh.geometry.clone();
  dilateGeometry(geometry, 0.05);
  var material = createAtmosphereMaterial();
  material.uniforms.glowColor.value = new THREE.Color("cyan");
  material.uniforms.coeficient.value = 0.1;
  material.uniforms.power.value = 1.2;

  material.side = THREE.BackSide;
  var outsideMesh = new THREE.Mesh(geometry, material);
  outsideMesh.scale.multiplyScalar(1.05);

  object3d.add(outsideMesh);

  // expose a few variable
  this.object3d = object3d;
  this.insideMesh = insideMesh;
  this.outsideMesh = outsideMesh;
};
