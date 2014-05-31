using System;

using iTextSharp.text.pdf.hyphenation;

/*
 * This file is part of the iText project.
 * Copyright (c) 1998-2014 iText Group NV
 * Authors: Bruno Lowagie, Paulo Soares, et al.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License version 3
 * as published by the Free Software Foundation with the addition of the
 * following permission added to Section 15 as permitted in Section 7(a):
 * FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
 * ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
 * OF THIRD PARTY RIGHTS
 *
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU Affero General Public License for more details.
 * You should have received a copy of the GNU Affero General Public License
 * along with this program; if not, see http://www.gnu.org/licenses or write to
 * the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
 * Boston, MA, 02110-1301 USA, or download the license from the following URL:
 * http://itextpdf.com/terms-of-use/
 *
 * The interactive user interfaces in modified source and object code versions
 * of this program must display Appropriate Legal Notices, as required under
 * Section 5 of the GNU Affero General Public License.
 *
 * In accordance with Section 7(b) of the GNU Affero General Public License,
 * a covered work must retain the producer line in every PDF that is created
 * or manipulated using iText.
 *
 * You can be released from the requirements of the license by purchasing
 * a commercial license. Buying such a license is mandatory as soon as you
 * develop commercial activities involving the iText software without
 * disclosing the source code of your own applications.
 * These activities include: offering paid services to customers as an ASP,
 * serving PDFs on the fly in a web application, shipping iText with a closed
 * source product.
 *
 * For more information, please contact iText Software Corp. at this
 * address: sales@itextpdf.com
 */

namespace iTextSharp.text.pdf {

    /** Hyphenates words automatically accordingly to the language and country.
     * The hyphenator engine was taken from FOP and uses the TEX patterns. If a language
     * is not provided and a TEX pattern for it exists, it can be easily adapted.
     *
     * @author Paulo Soares
     */
    public class HyphenationAuto : IHyphenationEvent {

        /** The hyphenator engine.
                                      */    
        protected Hyphenator hyphenator;
        /** The second part of the hyphenated word.
                                      */    
        protected string post;
    
        /** Creates a new hyphenation instance usable in <CODE>Chunk</CODE>.
        * @param lang the language ("en" for english, for example)
        * @param country the country ("GB" for Great-Britain or "none" for no country, for example)
        * @param leftMin the minimun number of letters before the hyphen
        * @param rightMin the minimun number of letters after the hyphen
        */    
        public HyphenationAuto(string lang, string country, int leftMin, int rightMin) {
            hyphenator = new Hyphenator(lang, country, leftMin, rightMin);
        }

        /** Gets the hyphen symbol.
         * @return the hyphen symbol
         */    
        virtual public string HyphenSymbol {
            get {
                return "-";
            }
        }
    
        /** Hyphenates a word and returns the first part of it. To get
         * the second part of the hyphenated word call <CODE>getHyphenatedWordPost()</CODE>.
         * @param word the word to hyphenate
         * @param font the font used by this word
         * @param fontSize the font size used by this word
         * @param remainingWidth the width available to fit this word in
         * @return the first part of the hyphenated word including
         * the hyphen symbol, if any
         */    
        virtual public string GetHyphenatedWordPre(string word, BaseFont font, float fontSize, float remainingWidth) {
            post = word;
            string hyphen = this.HyphenSymbol;
            float hyphenWidth = font.GetWidthPoint(hyphen, fontSize);
            if (hyphenWidth > remainingWidth)
                return "";
            Hyphenation hyphenation = hyphenator.Hyphenate(word);
            if (hyphenation == null) {
                return "";
            }
            int len = hyphenation.Length;
            int k;
            for (k = 0; k < len; ++k) {
                if (font.GetWidthPoint(hyphenation.GetPreHyphenText(k), fontSize) + hyphenWidth > remainingWidth)
                    break;
            }
            --k;
            if (k < 0)
                return "";
            post = hyphenation.GetPostHyphenText(k);
            return hyphenation.GetPreHyphenText(k) + hyphen;
        }
    
        /** Gets the second part of the hyphenated word. Must be called
         * after <CODE>getHyphenatedWordPre()</CODE>.
         * @return the second part of the hyphenated word
         */    
        virtual public string HyphenatedWordPost {
            get {
                return post;
            }
        }
    }
}
