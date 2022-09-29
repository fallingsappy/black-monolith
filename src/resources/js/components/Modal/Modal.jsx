import React from "react";
import "./Modal.css";

function Modal({ children, shown, close }) {
  return shown && (
    <div
      className="modal-backdrop"
      onClick={() => {
        // close modal when outside of modal is clicked
        close();
      }}
    >
      <div
        className="modal-content"
        onClick={(e) => {
          // do not close modal if anything inside modal content is clicked
          e.stopPropagation();
        }}
      >
        <div className="close-container">
          <span className="close" onClick={close}>
            +
          </span>
        </div>
        {children}
      </div>
    </div>
  );
}

export default Modal;
