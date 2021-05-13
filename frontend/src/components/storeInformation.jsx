import Rating from "./rating";
import React, { Component } from "react";
import { connect } from "react-redux";
import { fetchStoreInfo } from "../actions/storeActions";
import StoreSideBar from "./storeSideBar";

class StoreInformation extends Component {
  state = {};

  componentDidMount() {
    if (this.props.guid) {
      this.props.dispatch(fetchStoreInfo(this.props.guid));
    }
  }
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
    isAuthenticated: store.auth.isAuthenticated,
  };
};

export default connect(mapStateToProps)(StoreInformation);
