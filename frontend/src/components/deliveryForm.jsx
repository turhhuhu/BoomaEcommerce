import React, { Component } from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import { submitDeliveryInfo } from "../actions/userActions";
import { countriesArray } from "../utils/constants";

class DeliveryForm extends Component {
  state = {
    name: "",
    country: "",
    city: "",
    address: "",
    zip: "",
  };
  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };
  handleDelivery = () => {
    this.props.dispatch(submitDeliveryInfo(this.state));
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
                type="submit"
                className="btn btn-primary btn-block"
              >
                {" "}
                Continue to review purchase{" "}
              </button>
            </div>
          </form>
        </div>
      </div>
    );
  }
}

export default withRouter(connect()(DeliveryForm));
