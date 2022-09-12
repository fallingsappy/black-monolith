import React from "react";
import "./App.css";
import Main from "./resources/js/views/Main/Main";
import Footer from "./resources/js/views/Footer/Footer";

function App() {
  return (
    <div className="App">
      <Main />
      <Footer />
      <div className="scanlines"></div>
    </div>
  );
}

export default App;
