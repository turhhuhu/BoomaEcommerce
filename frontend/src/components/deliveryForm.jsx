import { Alert } from "@material-ui/lab";
import React, { Component } from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { submitDeliveryInfo } from "../actions/userActions";
import { countriesArray } from "../utils/constants";
import { isNormalInteger } from "../utils/utilFunctions";

class DeliveryForm extends Component {
  state = {
    error: undefined,
    name: this.props.deliveryInfo.name ? this.props.deliveryInfo.name : "",
    country: this.props.deliveryInfo.country
      ? this.props.deliveryInfo.country
      : "",
    city: this.props.deliveryInfo.city ? this.props.deliveryInfo.city : "",
    address: this.props.deliveryInfo.address
      ? this.props.deliveryInfo.address
      : "",
    zip: this.props.deliveryInfo.zip ? this.props.deliveryInfo.zip : "",
  };
  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  handleSkipDelivery = () => {
    this.props.history.push("/cart/purchase");
  };
  handleDelivery = () => {
    if (
      !this.state.name ||
      !this.state.country ||
      !this.state.city ||
      !this.state.address ||
      !this.state.zip
    ) {
      this.setState({ error: "Please fill out all fields" });
      return;
    }
    if (!isNormalInteger(this.state.zip)) {
      this.setState({ error: "zip must be a number" });
      return;
    }
    this.props.dispatch(
      submitDeliveryInfo({
        name: this.state.name,
        country: this.state.country,
        city: this.state.city,
        address: this.state.address,
        zip: this.state.zip,
      })
    );
    this.props.history.push("/cart/purchase");
  };

  render() {
    return (
      <div
        className="row col-3 mx-auto card"
        style={{
          marginTop: "75px",
        }}
      >
        <div className="card-body">
          <h4 className="card-title mb-4">Delivery info</h4>
          <form>
            <div className="form-row">
              <div className="col form-group">
                <label>Full name:</label>
                <input
                  type="text"
                  className="form-control"
                  name="name"
                  value={this.state.name}
                  onChange={this.handleChange}
                ></input>
              </div>
            </div>

            <div className="form-row">
              <div className="form-group col-md-6">
                <label>Country:</label>
                <select
                  onChange={this.handleChange}
                  className="form-control"
                  name="country"
                >
                  {countriesArray.map((country, index) =>
                    index === 0 ? (
                      <option hidden key={index}>
                        Choose...
                      </option>
                    ) : (
                      <option value={country} key={index}>
                        {country}
                      </option>
                    )
                  )}
                </select>
              </div>
              <div className="form-group col-md-6">
                <label>City:</label>
                <input
                  type="text"
                  className="form-control"
                  name="city"
                  value={this.state.city}
                  onChange={this.handleChange}
                ></input>
              </div>
            </div>
            <div className="form-row">
              <div className="form-group col-md-6">
                <label>Adress:</label>
                <input
                  type="text"
                  className="form-control"
                  name="address"
                  value={this.state.address}
                  onChange={this.handleChange}
                ></input>
              </div>
              <div className="form-group col-md-6">
                <label>Zip:</label>
                <input
                  type="text"
                  className="form-control"
                  name="zip"
                  value={this.state.zip}
                  onChange={this.handleChange}
                ></input>
              </div>
            </div>
            <div className="form-group">
              <button
                onClick={this.handleDelivery}
                type="button"
                className="btn btn-primary btn-block"
              >
                {" "}
                Continue to review purchase{" "}
              </button>
              <button
                onClick={this.handleSkipDelivery}
                type="button"
                className="btn btn-outline-secondary btn-block"
              >
                {" "}
                Skip delivery and review purchase{" "}
              </button>
              {(this.props.error || this.state.error) && (
                <div className="mt-2">
                  <Alert severity="error" onClick={() => this.setState(null)}>
                    {this.props.error || this.state.error}
                  </Alert>
                </div>
              )}
            </div>
          </form>
        </div>
      </div>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    deliveryInfo: store.user.deliveryInfo,
  };
};

export default withRouter(connect(mapStateToProps)(DeliveryForm));
