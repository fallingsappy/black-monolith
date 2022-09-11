import React from "react";
import "./resources/css/footer.css";

function Footer() {
  return (
    <div>
      <div className="footer">
        <span>sno som {new Date().getFullYear()}</span>
      </div>
      <div className="phantom" />
    </div>
  );
}

export default Footer;
