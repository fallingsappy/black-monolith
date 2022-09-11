import React from "react";
import Monolith from "../../components/Monolith/Monolith";
import Modal from "../../components/Modal/Modal";
import "./Main.css";

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
                <div className="wrapper">
                    <ul>
                        <li className="facebook">
                            <i className="fa fa-facebook" aria-hidden="true"></i>
                            <div className="slider">
                                <p>facebook</p>
                            </div>
                        </li>

                        <li className="twitter">
                            <i className="fa fa-twitter" aria-hidden="true"></i>
                            <div className="slider">
                                <p>twitter</p>
                            </div>
                        </li>

                        <li className="instagram">
                            <i className="fa fa-instagram" aria-hidden="true"></i>
                            <div className="slider">
                                <p>instagram</p>
                            </div>
                        </li>

                        <li className="google">
                            <i className="fa fa-google" aria-hidden="true"></i>
                            <div className="slider">
                                <p>google</p>
                            </div>
                        </li>

                        <li className="whatsapp">
                            <i className="fa fa-whatsapp" aria-hidden="true"></i>
                            <div className="slider">
                                <p>whatsapp</p>
                            </div>
                        </li>
                    </ul>
                </div>
            </Modal>
        </div>
    );
};

export default Main;
