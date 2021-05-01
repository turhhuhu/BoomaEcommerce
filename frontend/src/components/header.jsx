import React, { Component } from "react";
import { connect } from "react-redux";
import ecommerceLogo from "../images/ecommerce.png";
import "../css/navbar.css";
import CartIcon from "./cartIcon";
import UserIcon from "./userIcon";
import MessageIcon from "./messageIcon";
import NotificationIcon from "./notificationIcon";

class Header extends Component {
  state = {
    scrollbar: undefined,
  };

  handleChange = (event) => {
    console.log(this.state.scrollbar);
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  render() {
    return (
      <header className="section-header">
        <section className="header-main border-bottom">
          <div className="container" style={{ maxWidth: "1500px" }}>
            <div className="row align-items-center">
              <div className="col-lg-1 col-4">
                <a href="/home" className="brand-wrap">
                  <img
                    className="Logo"
                    alt=""
                    src={ecommerceLogo}
                    style={{ width: "70px" }}
                  ></img>
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
                    <select
                      onChange={this.handleChange}
                      className="custom-select"
                      name="scrollbar"
                    >
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
              <div className="widgets-wrap ml-5 float-md-right">
                <NotificationIcon />
                <MessageIcon />
                <CartIcon />
                <UserIcon />
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
