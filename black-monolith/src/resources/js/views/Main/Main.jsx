import React from "react";
import Monolith from "../../components/Monolith/Monolith";
import Modal from "../../components/Modal/Modal";
import SocialMediaLinks from "../SocialMediaLinks/SocialMediaLinks";

const Main = () => {
    const [modalShown, toggleModal] = React.useState(false);

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
