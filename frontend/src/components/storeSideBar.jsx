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
      <aside
        className={`ml-5 ${
          this.props.colClass ? this.props.colClass : "col-md-3"
        }`}
      >
        <ul className="list-group">
          <a
            className={`list-group-item ${
              this.props.isInformation ? "active" : null
            }`}
            href={`/store/${this.props.guid}`}
          >
            {" "}
            Information{" "}
          </a>
          <a
            className={`list-group-item ${
              this.props.isManagment ? "active" : null
            }`}
            href={`/store/${this.props.guid}/managment`}
          >
            {" "}
            Managment{" "}
          </a>
          <a
            className={`list-group-item ${
              this.props.isProducts ? "active" : null
            }`}
            href={`/store/${this.props.guid}/products`}
          >
            {" "}
            Products{" "}
          </a>
        </ul>
      </aside>
    );
  }
}

export default connect()(ProfileSideBar);
