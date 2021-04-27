import React, { Component } from "react";
import { connect } from "react-redux";
import "../css/product.css";
import ProductRating from "./productRating";

class Product extends Component {
  state = {};
  render() {
    return (
      <div class="col-md-4">
        <figure class="card card-product-grid">
          <figcaption class="info-wrap">
            <div class="fix-height">
              <label class="title product-card-item">ChessBoard</label>
              <p class="text-muted small product-card-item">
                Store: Ori's store
                <br />
                Description: An ChessBoard
              </p>
              <ProductRating />
              <div class="price-wrap mt-2 product-card-item">
                <label>Price:</label>
                <span class="price"> $1280</span>
                {/* <del class="price-old">$1980</del> */}
              </div>
              <div class="form-group col-md flex-grow-0">
                <label>Quantity:</label>
                <div class="input-group input-spinner">
                  <div class="input-group-prepend">
                    <button
                      class="btn btn-outline-primary"
                      type="button"
                      id="button-plus"
                    >
                      {" "}
                      +{" "}
                    </button>
                  </div>
                  <input type="text" class="form-control" value="1"></input>
                  <div class="input-group-append">
                    <button
                      class="btn btn-outline-primary"
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
            <button class="btn btn-outline-primary btn-block">
              {" "}
              Add to cart
              <i class="fa fa-shopping-cart"></i>
            </button>
          </figcaption>
        </figure>
      </div>
    );
  }
}

export default connect()(Product);
