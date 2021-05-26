import React, { Component } from "react";
import { connect } from "react-redux";
import "../css/profileSidebar.css";

class StoreSideBar extends Component {
  state = {};

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
          {this.props.myRole?.type === "ownership" ||
          (this.props.myRole?.type === "management" &&
            this.props.myRole?.permissions.canGetSellersInfo) ? (
            <a
              className={`list-group-item ${
                this.props.isManagement ? "active" : null
              }`}
              href={`/store/${this.props.guid}/management`}
            >
              {" "}
              Managment{" "}
            </a>
          ) : null}
          {this.props.myRole?.type === "ownership" ? (
            <a
              className={`list-group-item ${
                this.props.isPolicy ? "active" : null
              }`}
              href={`/store/${this.props.guid}/policy`}
            >
              {" "}
              Policy{" "}
            </a>
          ) : null}
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

const mapStateToProps = (store) => {
  return {
    myRole: store.user.storeRole,
  };
};

export default connect(mapStateToProps)(StoreSideBar);
