import React, { useState } from "react";
import Monolith from "../../components/Monolith/Monolith";
import Modal from "../../components/Modal/Modal";
import SocialMediaLinks from "../SocialMediaLinks/SocialMediaLinks";
import { useSearchParams } from "react-router-dom";
import AboutUs from "../AboutUs/AboutUs";
import "./Main.css";

const Main = () => {
  const [searchParams] = useSearchParams();
  const showLinks = searchParams.get("showLinks");

  const [modalShown, toggleModal] = useState(showLinks);
  const [showAboutUs, setShowAboutUs] = useState(false);

  return (
    <div className="body">
      <Monolith toggleModal={toggleModal} />
      <Modal
        shown={modalShown}
        close={() => {
          toggleModal(false);
        }}
      >
        <SocialMediaLinks />
      </Modal>
      <div onClick={() => setShowAboutUs(true)} className="about-us">
        About us
      </div>
      <Modal
        shown={showAboutUs}
        close={() => {
          setShowAboutUs(false);
        }}
      >
        <AboutUs />
      </Modal>
    </div>
  );
};

export default Main;
