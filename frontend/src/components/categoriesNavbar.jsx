import React, { Component } from "react";
import { connect } from "react-redux";

//TODO: add navigation scroll down functionality
class NavBar extends Component {
  state = {};
  render() {
    return (
      <nav className="navbar navbar-main navbar-expand-lg navbar-light border-bottom">
        <div className="container">
          <button
            className="navbar-toggler"
            type="button"
            data-toggle="collapse"
            data-target="#main_nav"
            aria-controls="main_nav"
            aria-expanded="false"
            aria-label="Toggle navigation"
          >
            <span className="navbar-toggler-icon"></span>
          </button>

          <div className="collapse navbar-collapse" id="main_nav">
            <ul className="navbar-nav">
              <li className="nav-item dropdown">
                <a
                  className="nav-link pl-0"
                  data-toggle="dropdown"
                  href="/login"
                  aria-expanded="false"
                >
                  <strong>
                    {" "}
                    <i className="fa fa-bars"></i> &nbsp; All categories
                  </strong>
                </a>
                {/* <div className="dropdown-menu">
                  <a className="dropdown-item" href="#">
                    Foods and Drink
                  </a>
                  <a className="dropdown-item" href="#">
                    Home interior
                  </a>
                </div> */}
              </li>
              <li className="nav-item">
                <a className="nav-link" href="/login">
                  Chess
                </a>
              </li>
            </ul>
          </div>
        </div>
      </nav>
    );
  }
}

export default connect()(NavBar);
