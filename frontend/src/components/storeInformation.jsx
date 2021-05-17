import Rating from "./rating";
import React, { Component } from "react";
import { connect } from "react-redux";
import StoreSideBar from "./storeSideBar";
import { Checkbox } from "@material-ui/core";

class StoreInformation extends Component {
  state = {};

  render() {
    return (
      <div className="ml-5 row">
        <StoreSideBar isInformation="true" guid={this.props.guid} />
        <main className="card col-md-6">
          <div className="card-body">
            <div className="border-bottom" style={{ height: "40px" }}>
              <h4 className="card-title mb-4">Store Information</h4>
            </div>
            <article>
              <div className="form-row mt-2">
                <div className="form-group col">
                  <strong className="bold">Name:</strong>
                  <p>{this.props.storeInfo.storeName}</p>
                </div>
              </div>

              <div className="form-row">
                <div className="form-group col">
                  <strong>Description:</strong>
                  <p>{this.props.storeInfo.description}</p>
                </div>
              </div>

              <div className="form-row">
                <div className="form-group col">
                  <strong>rating:</strong>
                  <div className="pr-4">
                    <Rating rating={this.props.storeInfo.rating} />
                  </div>
                </div>
              </div>

              <div className="form-row">
                <div className="form-group col">
                  <strong>Role:</strong>
                  <p>{this.props.storeRole?.type}</p>
                  {this.props.storeRole?.type === "management" ? (
                    <div>
                      {" "}
                      <strong>Permissions:</strong>
                      <div>
                        <label>Add product:</label>
                        <Checkbox color="primary" />
                        <br />
                        <label>Remove product:</label>
                        <Checkbox color="primary" />
                        <br />
                        <label>Delete product:</label>
                        <Checkbox color="primary" />
                        <br />
                        <label>View other sellers:</label>
                        <Checkbox color="primary" />
                      </div>{" "}
                    </div>
                  ) : null}
                </div>
              </div>
            </article>
          </div>
        </main>
      </div>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    storeInfo: store.store.storeInfo,
    storeRole: store.user.storeRole,
    isAuthenticated: store.auth.isAuthenticated,
  };
};

export default connect(mapStateToProps)(StoreInformation);
