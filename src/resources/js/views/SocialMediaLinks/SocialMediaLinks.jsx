import React from "react";
import "./SocialMediaLinks.css";
import { useWindowSize } from "../../../hooks/useWindowSize";

const SocialMediaLinks = () => {
  const size = useWindowSize();

  return (
    <ul>
      <li>
        <a
          href="https://www.instagram.com/sno_som/"
          target="_blank"
          rel="noopener noreferrer"
        >
          <div className="instagram">
            <div className="logo-container">
              <i className="fa fa-instagram" aria-hidden="true" />
            </div>
            {size.width > 1210 && (
              <div className="slider">
                <p>
                  THIS is our instagram{" "}
                  <span role="img" aria-label="emoji">
                    🙌
                  </span>
                  {"! "}
                  Check our latest reels and music videos{" "}
                  <span role="img" aria-label="emoji">
                    🌛
                  </span>
                </p>
              </div>
            )}
          </div>
        </a>
      </li>

      <li>
        <a
          href="https://t.me/sno_som"
          target="_blank"
          rel="noopener noreferrer"
        >
          <div className="telegram">
            <div className="logo-container">
              <i className="fa fa-telegram" aria-hidden="true" />
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
        </a>
      </li>

      <li>
        <a
          href="https://soundcloud.com/snosom"
          target="_blank"
          rel="noopener noreferrer"
        >
          <div className="soundcloud">
            <div className="logo-container">
              <i className="fa fa-soundcloud" aria-hidden="true" />
            </div>
            {size.width > 1210 && (
              <div className="slider">
                <p>
                  <span role="img" aria-label="emoji">
                    🧛‍♀
                  </span>
                  ️Our latest tracks{" "}
                  <span role="img" aria-label="emoji">
                    👼
                  </span>
                </p>
              </div>
            )}
          </div>
        </a>
      </li>

      <li>
        {/*<a*/}
        {/*  */}
        {/*  href="https://snosom.bandcamp.com/"*/}
        {/*  target="_blank"*/}
        {/*  rel="noopener noreferrer"*/}
        {/*>*/}
        <div style={{ opacity: "0.4" }} className="bandcamp">
          <div className="logo-container">
            <i className="fa fa-bandcamp" aria-hidden="true" />
          </div>
          {size.width > 1210 && (
            <div className="slider">
              <p>Coming soon!</p>
              {/*<p>*/}
              {/*  Liza is a mega writing{" "}*/}
              {/*  <span role="img" aria-label="emoji">*/}
              {/*    ✍*/}
              {/*  </span>*/}
              {/*  ️ machine,{" "}*/}
              {/*  <span role="img" aria-label="emoji">*/}
              {/*    🤖*/}
              {/*  </span>{" "}*/}
              {/*  so check{" "}*/}
              {/*  <span role="img" aria-label="emoji">*/}
              {/*    ✔*/}
              {/*  </span>*/}
              {/*  ️ our{" "}*/}
              {/*  <span role="img" aria-label="emoji">*/}
              {/*    😌*/}
              {/*  </span>*/}
              {/*  <span role="img" aria-label="emoji">*/}
              {/*    💰*/}
              {/*  </span>*/}
              {/*  <span role="img" aria-label="emoji">*/}
              {/*    🔥*/}
              {/*  </span>{" "}*/}
              {/*  lyrics{" "}*/}
              {/*  <span role="img" aria-label="emoji">*/}
              {/*    🎵*/}
              {/*  </span>*/}
              {/*</p>*/}
            </div>
          )}
        </div>
        {/*</a>*/}
      </li>

      <li>
        <a
          href="mailto: messageus@snosom.com"
          target="_blank"
          rel="noopener noreferrer"
        >
          <div className="email">
            <div className="logo-container">
              <i className="fa fa-envelope" aria-hidden="true" />
            </div>
            {size.width > 1210 && (
              <div className="slider">
                <p>
                  Write{" "}
                  <span role="img" aria-label="emoji">
                    📝
                  </span>{" "}
                  to us{" "}
                  <span role="img" aria-label="emoji">
                    👨
                  </span>{" "}
                  and don't{" "}
                  <span role="img" aria-label="emoji">
                    🚖
                  </span>
                  <span role="img" aria-label="emoji">
                    🔱
                  </span>{" "}
                  be{" "}
                  <span role="img" aria-label="emoji">
                    🌼
                  </span>{" "}
                  shy,{" "}
                  <span role="img" aria-label="emoji">
                    🙅‍
                  </span>
                  <span role="img" aria-label="emoji">
                    ♀
                  </span>
                  ️
                  <span role="img" aria-label="emoji">
                    😳
                  </span>{" "}
                  because{" "}
                  <span role="img" aria-label="emoji">
                    🐕
                  </span>{" "}
                  we{" "}
                  <span role="img" aria-label="emoji">
                    🌿
                  </span>{" "}
                  are already{" "}
                  <span role="img" aria-label="emoji">
                    😃
                  </span>{" "}
                  shy{" "}
                  <span role="img" aria-label="emoji">
                    💬
                  </span>
                </p>
              </div>
            )}
          </div>
        </a>
      </li>

      <li>
        {/*<a*/}
        {/*  */}
        {/*  rel="noopener noreferrer"*/}
        {/*  // href="https://www.youtube.com/channel/UCG1qeXJZhB96GisK_tF2jUA"*/}
        {/*  href="/"*/}
        {/*  target="_blank"*/}
        {/*>*/}
        <div style={{ opacity: "0.4" }} className="youtube">
          <div className="logo-container">
            <i className="fa fa-youtube" aria-hidden="true" />
          </div>
          {size.width > 1210 && (
            <div className="slider">
              <p>Coming soon!</p>
              {/*<p>*/}
              {/*  The place{" "}*/}
              {/*  <span role="img" aria-label="emoji">*/}
              {/*    🏆*/}
              {/*  </span>*/}
              {/*  <span role="img" aria-label="emoji">*/}
              {/*    🏆*/}
              {/*  </span>*/}
              {/*  <span role="img" aria-label="emoji">*/}
              {/*    🏆*/}
              {/*  </span>{" "}*/}
              {/*  where{" "}*/}
              {/*  <span role="img" aria-label="emoji">*/}
              {/*    🌎*/}
              {/*  </span>*/}
              {/*  <span role="img" aria-label="emoji">*/}
              {/*    🤷*/}
              {/*  </span>{" "}*/}
              {/*  we{" "}*/}
              {/*  <span role="img" aria-label="emoji">*/}
              {/*    😉*/}
              {/*  </span>{" "}*/}
              {/*  try{" "}*/}
              {/*  <span role="img" aria-label="emoji">*/}
              {/*    ✏*/}
              {/*  </span>*/}
              {/*  ️ to avoid{" "}*/}
              {/*  <span role="img" aria-label="emoji">*/}
              {/*    👋*/}
              {/*  </span>{" "}*/}
              {/*  copyright{" "}*/}
              {/*  <span role="img" aria-label="emoji">*/}
              {/*    ©*/}
              {/*  </span>{" "}*/}
              {/*  issues*/}
              {/*</p>*/}
            </div>
          )}
        </div>
        {/*</a>*/}
      </li>

      {/*<li>*/}
      {/*  <a*/}
      {/*    href="#"*/}
      {/*    target="_blank"*/}
      {/*    rel="noopener noreferrer"*/}
      {/*  >*/}
      {/*    <div className="zaycev">*/}
      {/*      <div className="logo-container">*/}
      {/*        <img style={{ height: "50px" }} src={zaycev} />*/}
      {/*      </div>*/}
      {/*      {size.width > 1210 && (*/}
      {/*        <div className="slider">*/}
      {/*          <p>*/}
      {/*            <span role="img" aria-label="emoji">*/}
      {/*              🐇*/}
      {/*            </span>*/}
      {/*            .net, but there is sno_som*/}
      {/*          </p>*/}
      {/*        </div>*/}
      {/*      )}*/}
      {/*    </div>*/}
      {/*  </a>*/}
      {/*</li>*/}
    </ul>
  );
};

export default SocialMediaLinks;
