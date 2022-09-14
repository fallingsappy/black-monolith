import React from "react";
import Monolith from "../../components/Monolith/Monolith";
import Modal from "../../components/Modal/Modal";
import SocialMediaLinks from "../SocialMediaLinks/SocialMediaLinks";
import { useSearchParams } from "react-router-dom";

const Main = () => {
  const [ searchParams ] = useSearchParams()
  const showLinks = searchParams.get('showLinks'); // "testCode"

  const [modalShown, toggleModal] = React.useState(showLinks);

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
    </div>
  );
};

export default Main;
