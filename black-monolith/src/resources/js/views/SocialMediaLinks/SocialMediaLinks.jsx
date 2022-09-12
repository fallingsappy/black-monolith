import React from "react";
import "./SocialMediaLinks.css";

const SocialMediaLinks = () => {
    return (
        <div className="wrapper">
            <ul>
                <li className="instagram">
                    <i className="fa fa-instagram" aria-hidden="true"></i>
                    <div className="slider">
                        <p>Заходи в наш instagram и посмотри 👁️ как наши треки ⏭️ создаются 👉 из первых 🌛 рук 🙌</p>
                    </div>
                </li>


                <li className="telegram">
                    <i className="fa fa-telegram" aria-hidden="true"></i>
                    <div className="slider">
                        <p>Check ✅ our telegram for 🈺 the 🤣🤘 latest updates and nasty sneak peaks</p>
                    </div>
                </li>
            </ul>
        </div>
    );
};

export default SocialMediaLinks;
