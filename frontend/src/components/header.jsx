import React, { Component } from "react";
import { connect } from "react-redux";
import bootstrapIcon from "../images/logo.png";
import "../css/navbar.css";

class Header extends Component {
  state = {};
  render() {
    return (
      <header className="section-header">
        <section className="header-main border-bottom">
          <div className="container">
            <div className="row align-items-center">
              <div className="col-lg-2 col-4">
                <a href="/login" className="brand-wrap">
                  <img className="Logo" alt="" src={bootstrapIcon}></img>
                </a>
              </div>
              <div className="col-lg-6 col-sm-12">
                <form action="#" className="search">
                  <div className="input-group w-100">
                    <input
                      type="text"
                      className="form-control"
                      placeholder="Search"
                    ></input>
                    <div className="input-group-append">
                      <button className="btn btn-primary" type="submit">
                        <i className="fa fa-search"></i>
                      </button>
                    </div>
                  </div>
                </form>
              </div>
              <div className="col-lg-4 col-sm-6 col-12">
                <div className="widgets-wrap float-md-right">
                  <div className="widget-header  mr-3">
                    <a
                      href="/login"
                      className="icon icon-sm rounded-circle border"
                    >
                      <i className="fa fa-shopping-cart"></i>
                    </a>
                    <span className="badge badge-pill badge-danger notify">
                      0
                    </span>
                  </div>
                  <div className="widget-header icontext">
                    <a
                      href="/login"
                      className="icon icon-sm rounded-circle border"
                    >
                      <i className="fa fa-user"></i>
                    </a>
                    <div className="text">
                      <span className="text-muted">Welcome!</span>
                      <div>
                        <a href="/login">Sign in</a> |
                        <a href="/register"> Register</a>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </section>
      </header>
    );
  }
}

export default connect()(Header);
