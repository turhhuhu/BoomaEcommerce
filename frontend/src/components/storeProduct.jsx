import React, { Component } from "react";
import { connect } from "react-redux";
import Rating from "./rating";

class StoreProduct extends Component {
  state = {};
  render() {
    return (
      <div className="col-md-4">
        <figure className="card card-product-grid">
          <figcaption className="info-wrap">
            <div className="fix-height">
              <h6 className="title product-card-item">{this.props.name}</h6>
              <p className="text-muted small product-card-item">
                Amount in store: {this.props.amount}
                <br />
                Category: {this.props.category}
              </p>
              <div className="product-card-item">
                <Rating rating={this.props.rating} />
              </div>
              <div className="price-wrap mt-2 product-card-item">
                <label>Price per unit:</label>
                <span className="price"> {this.props.price}</span>
                {/* <del className="price-old">$1980</del> */}
              </div>
            </div>
          </figcaption>
        </figure>
      </div>
    );
  }
}

export default connect()(StoreProduct);
