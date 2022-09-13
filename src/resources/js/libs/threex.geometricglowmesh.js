import * as THREE from "three";
import { createAtmosphereMaterial } from "./threex.atmospherematerial";
import { dilateGeometry } from "./threex.dilategeometry";

export const GeometricGlowMesh = function (mesh, addInner = false) {
  const object3d = new THREE.Object3D();

  const innerGeometry = mesh.geometry.clone();
  dilateGeometry(innerGeometry, 0.01);
  const innerMaterial = createAtmosphereMaterial();
  innerMaterial.uniforms.glowColor.value = new THREE.Color("cyan");
  innerMaterial.uniforms.coeficient.value = 1.1;
  innerMaterial.uniforms.power.value = 1.4;
  const insideMesh = new THREE.Mesh(innerGeometry, innerMaterial);

  if (addInner) {
    object3d.add(insideMesh);
  }

  const geometry = mesh.geometry.clone();
  dilateGeometry(geometry, 0.05);
  const material = createAtmosphereMaterial();
  material.uniforms.glowColor.value = new THREE.Color("cyan");
  material.uniforms.coeficient.value = 0.1;
  material.uniforms.power.value = 1.2;

  material.side = THREE.BackSide;
  const outsideMesh = new THREE.Mesh(geometry, material);
  outsideMesh.scale.multiplyScalar(1.05);

  object3d.add(outsideMesh);

  // expose a few variable
  this.object3d = object3d;
  this.insideMesh = insideMesh;
  this.outsideMesh = outsideMesh;
};
