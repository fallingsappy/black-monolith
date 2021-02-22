import React from "react";
import Header from "./Header";
import './resources/css/rotator.css'
import Footer from "./Footer";

import Monolith from "./resources/js/components/Monolith/Monolith";

function Layout () {
    return (
      <>
        <Header/>
        <Monolith/>
        <Footer/>
      </>
    )
}

export default Layout;
