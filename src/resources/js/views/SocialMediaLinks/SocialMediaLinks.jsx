import React from "react";
import "./SocialMediaLinks.css";
import { useWindowSize } from "../../../hooks/useWindowSize";

const SocialMediaLinks = () => {
  const size = useWindowSize();

  return (
    <div className="wrapper">
      <ul>
        <li>
          <div className="instagram">
            <div className="logo-container">
              <a href="https://www.instagram.com/sno_som/" target="_blank">
                <i className="fa fa-instagram" aria-hidden="true" />
              </a>
            </div>
            {size.width > 1210 && (
              <div className="slider">
                <p>
                  Заходи в наш instagram и посмотри{" "}
                  <span role="img" aria-label="emoji">
                    👁️
                  </span>{" "}
                  как наши треки{" "}
                  <span role="img" aria-label="emoji">
                    ⏭
                  </span>
                  ️создаются
                  <span role="img" aria-label="emoji">
                    👉
                  </span>{" "}
                  из первых{" "}
                  <span role="img" aria-label="emoji">
                    🌛
                  </span>{" "}
                  рук{" "}
                  <span role="img" aria-label="emoji">
                    🙌
                  </span>
                </p>
              </div>
            )}
          </div>
        </li>

        <li>
          <div className="telegram">
            <div className="logo-container">
              <a href="https://t.me/sno_som" target="_blank">
                <i className="fa fa-telegram" aria-hidden="true" />
              </a>
            </div>
            {size.width > 1210 && (
              <div className="slider">
                <p>
                  Check{" "}
                  <span role="img" aria-label="emoji">
                    ✅
                  </span>{" "}
                  our telegram for{" "}
                  <span role="img" aria-label="emoji">
                    🈺
                  </span>{" "}
                  the{" "}
                  <span role="img" aria-label="emoji">
                    🤣
                  </span>
                  <span role="img" aria-label="emoji">
                    🤘
                  </span>{" "}
                  latest updates and nasty sneak peaks
                </p>
              </div>
            )}
          </div>
        </li>
      </ul>
    </div>
  );
};

export default SocialMediaLinks;
