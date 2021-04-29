import React, { Component } from "react";
import { connect } from "react-redux";
import bootstrapIcon from "../images/logo.png";
import "../css/navbar.css";
import CartIcon from "./cartIcon";
import UserIcon from "./userIcon";
import MessageIcon from "./messageIcon";
import NotificationIcon from "./notificationIcon";

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
                      style={{ width: "55%" }}
                    ></input>
                    <select className="custom-select" name="category_name">
                      <option value="Keyword">Keyword</option>
                      <option value="Categories">Categories</option>
                      <option value="Name">Name</option>
                    </select>
                    <div className="input-group-append">
                      <button className="btn btn-primary" type="submit">
                        <i className="fa fa-search"></i>
                      </button>
                    </div>
                  </div>
                </form>
              </div>
              <div className="col-lg-4 col-sm-6 col-12">
                <div className="widgets-wrap float-md-right row">
                  <NotificationIcon />
                  <MessageIcon />
                  <CartIcon />
                  <UserIcon />
                </div>
              </div>
            </div>
          </div>
        </section>
      </header>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    isAuthenticated: store.auth.isAuthenticated,
    username: store.auth.username,
  };
};

export default connect(mapStateToProps)(Header);
