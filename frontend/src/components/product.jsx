import React, { Component } from "react";
import { connect } from "react-redux";
import "../css/product.css";
import ProductRating from "./productRating";

class Product extends Component {
  state = {};

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  render() {
    return (
      <div className="col-md-4">
        <figure className="card card-product-grid">
          <figcaption className="info-wrap">
            <div className="fix-height">
              <h6 className="title product-card-item">{this.props.name}</h6>
              <p className="text-muted small product-card-item">
                Store: Ori's store
                <br />
                Amount in store: {this.props.amount}
                <br />
                Category: {this.props.category}
              </p>
              <ProductRating rating={this.props.rating} />
              <div className="price-wrap mt-2 product-card-item">
                <label>Price:</label>
                <span className="price"> {this.props.price}</span>
                {/* <del className="price-old">$1980</del> */}
              </div>
              <div className="form-group col-md flex-grow-0">
                <label>Quantity:</label>
                <div className="input-group input-spinner">
                  <div className="input-group-prepend">
                    <button
                      className="btn btn-outline-primary"
                      type="button"
                      id="button-plus"
                    >
                      {" "}
                      +{" "}
                    </button>
                  </div>
                  <input
                    type="text"
                    className="form-control"
                    onChange={this.handleChange}
                    value={"1"}
                  ></input>
                  <div className="input-group-append">
                    <button
                      className="btn btn-outline-primary"
                      type="button"
                      id="button-minus"
                    >
                      {" "}
                      −{" "}
                    </button>
                  </div>
                </div>
              </div>
            </div>
            <button className="btn btn-outline-primary btn-block">
              {" "}
              Add to cart
              <i className="fa fa-shopping-cart"></i>
            </button>
          </figcaption>
        </figure>
      </div>
    );
  }
}

export default connect()(Product);
