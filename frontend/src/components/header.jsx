import React, { Component } from "react";
import { connect } from "react-redux";
import ecommerceLogo from "../images/ecommerce.png";
import "../css/navbar.css";
import CartIcon from "./cartIcon";
import UserIcon from "./userIcon";
// import MessageIcon from "./messageIcon";
import NotificationIcon from "./notificationIcon";
import { fetchAllProducts } from "../actions/productsActions";
import { withRouter } from "react-router";

class Header extends Component {
  state = {
    scrollbar: "keyword",
    searchBar: "",
  };

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  handleSearch = (event) => {
    event.preventDefault();
    if (!this.state.searchBar) {
      return;
    }
    this.props.dispatch(
      fetchAllProducts({ [this.state.scrollbar]: this.state.searchBar })
    );
    this.props.history.push("/products");
  };

  render() {
    return (
      <header className="section-header">
        <section className="header-main border-bottom">
          <div className="container" style={{ maxWidth: "1300px" }}>
            <div className="row align-items-center ml-3">
              <div className="col offset-5 ml-4">
                <a href="/home" className="brand-wrap">
                  <img
                    className="Logo"
                    alt=""
                    src={ecommerceLogo}
                    style={{ width: "70px" }}
                  ></img>
                </a>
              </div>
              <div className="col-7">
                <form action="#" className="search">
                  <div className="input-group w-100">
                    <input
                      type="text"
                      value={this.state.searchBar}
                      onChange={this.handleChange}
                      name="searchBar"
                      className="form-control"
                      placeholder="Search"
                      style={{ width: "55%" }}
                      required
                    ></input>
                    <select
                      onChange={this.handleChange}
                      className="custom-select"
                      name="scrollbar"
                    >
                      <option value="keyword">Keyword</option>
                      <option value="category">Category</option>
                      <option value="productName">Name</option>
                    </select>
                    <div className="input-group-append">
                      <button
                        onClick={this.handleSearch}
                        className="btn btn-primary"
                        type="submit"
                      >
                        <i className="fa fa-search"></i>
                      </button>
                    </div>
                  </div>
                </form>
              </div>
              <div className="widgets-wrap ml-5 float-md-right">
                <NotificationIcon />
                {/* <MessageIcon /> */}
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

export default withRouter(connect(mapStateToProps)(Header));
