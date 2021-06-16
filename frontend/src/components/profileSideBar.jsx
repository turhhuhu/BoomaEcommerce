import React, { Component } from "react";
import { connect } from "react-redux";
import { logoutUser } from "../actions/authActions";
import "../css/profileSidebar.css";
class ProfileSideBar extends Component {
  state = {};

  handleSignOut = () => {
    this.props.dispatch(logoutUser());
  };

  render() {
    return (
      <aside className="ml-5 col-md-3">
        <ul className="list-group">
          <a
            className={`list-group-item ${
              this.props.isProfile ? "active" : null
            }`}
            href="/user"
          >
            {" "}
            Profile{" "}
          </a>
          <a
            className={`list-group-item ${
              this.props.isStores ? "active" : null
            }`}
            href="/user/stores"
          >
            {" "}
            Stores{" "}
          </a>
          <a
            className={`list-group-item ${
              this.props.isPurchaseHistory ? "active" : null
            }`}
            href="/user/purchases"
          >
            {" "}
            Purchase history{" "}
          </a>
          <a
            className={`list-group-item ${
              this.props.isProductOffers ? "active" : null
            }`}
            href="/user/offers"
          >
            {" "}
            Product offers{" "}
          </a>
        </ul>
        <br />
        <a
          className="btn btn-light btn-block"
          href="/login"
          onClick={this.handleSignOut}
        >
          {" "}
          <i className="fa fa-power-off" style={{ color: "#969696" }}></i>{" "}
          <span className="text">Sign out</span>{" "}
        </a>
      </aside>
    );
  }
}

export default connect()(ProfileSideBar);
